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

        TypeDefinition entityZombie = gameModule.Types.First(type => type.Name.Equals("EntityZombie"));
        MethodDefinition dropCorpseBlockMethod = entityZombie.Methods.First(method => method.Name.Equals("dropCorpseBlock"));

        MethodDefinition baseDropCorpseBlockMethod = gameModule.Types.First(type => type.Name.Equals("EntityAlive")).Methods.First(method => method.Name.Equals("dropCorpseBlock"));

        Instruction instruction = dropCorpseBlockMethod.Body.Instructions.First(inst => inst.OpCode.Equals(OpCodes.Call) && inst.Operand.Equals(baseDropCorpseBlockMethod));

        TypeDefinition entity = gameModule.Types.First(type => type.Name.Equals("Entity"));
        FieldDefinition positionField = entity.Fields.First(field => field.Name.Equals("position"));
        FieldDefinition worldField = entity.Fields.First(field => field.Name.Equals("world"));

        TypeDefinition corpsePositionUpdater = modModule.Types.First(type => type.Name.Equals("ZombieCorpsePositionUpdater"));
        MethodReference getUpdatedPosition = gameModule.Import(corpsePositionUpdater.Methods.First(method => method.Name.Equals("GetUpdatedPosition")));

        ILProcessor processor = dropCorpseBlockMethod.Body.GetILProcessor();

        Instruction previousInstruction = Instruction.Create(instruction.Previous.OpCode);
        previousInstruction.Operand = instruction.Previous.Operand;

        instruction.Previous.OpCode = OpCodes.Ldarg_0;
        processor.InsertAfter(instruction.Previous, previousInstruction);
        
        processor.InsertBefore(previousInstruction, Instruction.Create(OpCodes.Ldarg_0));
        processor.InsertBefore(previousInstruction, Instruction.Create(OpCodes.Ldfld, worldField));
        processor.InsertBefore(previousInstruction, Instruction.Create(OpCodes.Ldarg_0));
        processor.InsertBefore(previousInstruction, Instruction.Create(OpCodes.Ldfld, positionField));
        processor.InsertBefore(previousInstruction, Instruction.Create(OpCodes.Call, getUpdatedPosition));
        processor.InsertBefore(previousInstruction, Instruction.Create(OpCodes.Stfld, positionField));
        return true;
    }
}