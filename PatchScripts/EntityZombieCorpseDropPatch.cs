using SDX.Compiler;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Linq;

public class EntityZombieCorpseDropPatch : IPatcherMod
{
    public bool Patch(ModuleDefinition module)
    {
        return true;
    }

    public bool Link(ModuleDefinition gameModule, ModuleDefinition modModule)
    {
        SDX.Core.Logging.LogInfo("Linking corpse fix module...");

        MethodDefinition dropCorpseBlockMethod = GetCorpseDropMethodToAlter(gameModule);
        Instruction callToBaseCorpseDropMethod = FindCallToBaseCorpseDropMethod(gameModule, dropCorpseBlockMethod);

        ILProcessor processor = dropCorpseBlockMethod.Body.GetILProcessor();

        Instruction copyOfExistingInstruction = OverwriteAndReAddInstruction(processor, callToBaseCorpseDropMethod.Previous, Instruction.Create(OpCodes.Ldarg_0));

        AddRemainingInstructionsBefore(processor, copyOfExistingInstruction, gameModule, modModule);

        return true;
    }

    private void MakeBlockImplementIBlock(ModuleDefinition gameModule, ModuleDefinition modModule)
    {
        TypeDefinition blockClass = gameModule.Types.First(type => type.Name.Equals("Block"));
        TypeDefinition iblockInterface = modModule.Types.First(type => type.Name.Equals("IBlock"));

        blockClass.Interfaces.Add(iblockInterface);
    }

    private MethodDefinition GetCorpseDropMethodToAlter(ModuleDefinition gameModule)
    {
        TypeDefinition entityZombie = gameModule.Types.First(type => type.Name.Equals("EntityZombie"));
        return entityZombie.Methods.First(method => method.Name.Equals("dropCorpseBlock"));
    }

    private Instruction FindCallToBaseCorpseDropMethod(ModuleDefinition gameModule, MethodDefinition corpseDropMethodToAlter)
    {
        MethodDefinition baseDropCorpseBlockMethod = gameModule.Types.First(type => type.Name.Equals("EntityAlive")).Methods.First(method => method.Name.Equals("dropCorpseBlock"));

        return corpseDropMethodToAlter.Body.Instructions.First(inst => inst.OpCode.Equals(OpCodes.Call) && inst.Operand.Equals(baseDropCorpseBlockMethod));
    }

    private FieldDefinition GetPositionField(ModuleDefinition gameModule)
    {
        TypeDefinition entity = gameModule.Types.First(type => type.Name.Equals("Entity"));
        return entity.Fields.First(field => field.Name.Equals("position"));
    }

    private MethodReference ImportCorpsePositionUpdaterIntoGameModule(ModuleDefinition gameModule, ModuleDefinition modModule)
    {
        TypeDefinition corpsePositionUpdater = modModule.Types.First(type => type.Name.Equals("ZombieCorpsePositionUpdater"));
        return gameModule.Import(corpsePositionUpdater.Methods.First(method => method.Name.Equals("GetUpdatedPosition")));
    }

    private Instruction CreateCopyOfInstruction(Instruction instruction)
    {
        Instruction copyOfExistinInstruction = Instruction.Create(instruction.OpCode);
        copyOfExistinInstruction.Operand = instruction.Operand;

        return copyOfExistinInstruction;
    }

    private void ReplaceInstruction(Instruction existingInstruction, Instruction newInstruction)
    {
        existingInstruction.OpCode = newInstruction.OpCode;
        existingInstruction.Operand = newInstruction.Operand;
    }

    private Instruction OverwriteAndReAddInstruction(ILProcessor processor, Instruction instructionToReplace, Instruction newInstruction)
    {
        Instruction copyOfExistingInstruction = CreateCopyOfInstruction(instructionToReplace);
        ReplaceInstruction(instructionToReplace, newInstruction);
        processor.InsertAfter(instructionToReplace, copyOfExistingInstruction);

        return copyOfExistingInstruction;
    }

    private void AddRemainingInstructionsBefore(ILProcessor processor, Instruction instruction, ModuleDefinition gameModule, ModuleDefinition modModule)
    {
        MethodReference getUpdatedPosition = ImportCorpsePositionUpdaterIntoGameModule(gameModule, modModule);
        FieldDefinition positionField = GetPositionField(gameModule);

        processor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldarg_0));
        processor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldfld, positionField));
        processor.InsertBefore(instruction, Instruction.Create(OpCodes.Call, getUpdatedPosition));
        processor.InsertBefore(instruction, Instruction.Create(OpCodes.Stfld, positionField));
    }
}