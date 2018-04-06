using SDX.Compiler;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Linq;

/// <summary>
/// This is a patch script that modifies some code in the stock 7D2D dll files. See https://7d2dsdx.github.io/Tutorials/index.html for more info.
/// </summary>
public class EntityZombieCorpseDropPatch : IPatcherMod
{
    /// <summary>
    /// This method is called to make alterations to the core game assembly before linking the game assembly with the mod assembly.
    /// <para>
    /// This mod requires the corpse block field of EntityAlive to have at least protected level access.
    /// </para>
    /// </summary>
    /// <param name="module">The core module for 7D2D.</param>
    /// <returns>True, always.</returns>
    public bool Patch(ModuleDefinition module)
    {
        FieldDefinition corpseBlockField = GetCorpseBlockField(module);
        SetFieldToProtected(corpseBlockField);

        return true;
    }

    private void SetFieldToProtected(FieldDefinition field)
    {
        if(!field.IsFamily && !field.IsPublic)
        {
            field.IsPrivate = false;
            field.IsFamily = true;
            field.IsPublic = false;
        }
    }

    /// <summary>
    /// This method is used to link the core 7D2D assembly with the compiled mod assembly. This is where the modifications are made to cause the core game to call the code for this mod.
    /// <para>
    /// The general strategy for making this mod work, is to have the code responsible for spawning a corpse block for a dead zombie, call this mod with its current position in the game world.
    /// This mod will then calculate a reasonable spot to spawn the corpse block and return the result to the calling code that's managing the dead zombie. The calling code will then update the
    /// position of the dead zombie, just before it calls the code to spawn the corpse block, the rest happens as normal.
    /// </para>
    /// <para>
    /// The code changes are made to <code>EntityZombie</code> which is the base class for all zombie types in the game. After modifications it should look something like this:
    /// </para>
    /// <code>
    /// <para>protected override Vector3i dropCorpseBlock()</para>
    /// <para>{</para>
    /// <para>...</para>
    /// <para>this.position = ZombieCorpsePositionUpdater.GetUpdatedPosition(this.position);</para>
	/// <para>Vector3i vector3i = base.dropCorpseBlock();</para>
    /// <para>...</para>
    /// <para>}</para>
    /// </code>
    /// </summary>
    /// <param name="gameModule">The core game module for 7D2D.</param>
    /// <param name="modModule">The module for this mod.</param>
    /// <returns>True, if it manages to complete.</returns>
    public bool Link(ModuleDefinition gameModule, ModuleDefinition modModule)
    {
        SDX.Core.Logging.LogInfo("Linking corpse fix module...");

        MethodDefinition dropCorpseBlockMethod = GetCorpseDropMethodToAlter(gameModule);
        Instruction callToBaseCorpseDropMethod = FindCallToBaseCorpseDropMethod(gameModule, dropCorpseBlockMethod);

        ILProcessor processor = dropCorpseBlockMethod.Body.GetILProcessor();

        /// There is some awkwardness here, because the call to this module's code, needs to be made in front of the first line after a closing conditional.
        /// If new instructions are directly added before the existing line at this position, the new instructions will be skipped over by the conditional.
        /// The strategy used here to compensate, is to record the existing instruction, and alter it to suit our needs, then re-add the existing instruction
        /// after the new block of instructions we are inserting has been executed.
        Instruction copyOfExistingInstruction = OverwriteAndReAddInstruction(processor, callToBaseCorpseDropMethod.Previous, Instruction.Create(OpCodes.Ldarg_0));

        AddRemainingInstructionsBefore(processor, copyOfExistingInstruction, gameModule, modModule);

        return true;
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
        FieldDefinition corpseBlockField = GetCorpseBlockField(gameModule);

        processor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldarg_0));
        processor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldfld, positionField));
        processor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldarg_0));
        processor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldfld, corpseBlockField));
        processor.InsertBefore(instruction, Instruction.Create(OpCodes.Call, getUpdatedPosition));
        processor.InsertBefore(instruction, Instruction.Create(OpCodes.Stfld, positionField));
    }

    private FieldDefinition GetCorpseBlockField(ModuleDefinition gameModule)
    {
        TypeDefinition entity = gameModule.Types.First(type => type.Name.Equals("EntityAlive"));
        return entity.Fields.First(field => field.Name.Equals("XH"));
    }
}