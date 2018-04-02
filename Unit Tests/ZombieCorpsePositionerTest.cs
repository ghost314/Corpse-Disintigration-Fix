using NUnit.Framework;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[TestFixture]
public class ZombieCorpsePositionerTest
{
    private const int WORLD_HEIGHT = 300;
    private const int WORLD_LENGTH = 400;
    private const int GROUND_HEIGHT = 150;
    private ZombieCorpsePositioner positioner;
    private IBlock[,,] fakeWorld = new IBlock[WORLD_LENGTH, WORLD_HEIGHT, WORLD_LENGTH];

    [SetUp]
    public void SetUp()
    {
        positioner = new ZombieCorpsePositioner(Console.WriteLine, GetBlockAt);
        
        for (int x = 0; x < WORLD_LENGTH; x++)
        {
            for (int y = 0; y < WORLD_HEIGHT; y++)
            {
                for (int z = 0; z < WORLD_LENGTH; z++)
                {
                    IBlock nextBlock;
                    if (y <= GROUND_HEIGHT)
                        nextBlock = GenerateOccupiedBlock();
                    else
                        nextBlock = GenerateEmptyBlock();
                    fakeWorld[x, y, z] = nextBlock;
                }
            }
        }
    }

    [Test]
    public void WhenCurrentPositionIsAlreadyEmptyThenCorpseIsSpawnedAtCurrentPosition()
    {
        int height = GROUND_HEIGHT + 1;

        Vector3i position = positioner.FindSpawnLocationStartingFrom(new Vector3i(50, height, 50));

        Assert.AreEqual(new Vector3i(50, height, 50), position);
    }

    [Test]
    public void WhenCurrentPositionIsBelowGroundThenCorpseIsSpawnedAboveGround()
    {
        Vector3i position = positioner.FindSpawnLocationStartingFrom(new Vector3i(50, 25, 50));

        Assert.AreEqual(new Vector3i(50, GROUND_HEIGHT + 1, 50), position);
    }

    [Test]
    public void WhenCurrentPositionIsMidAirThenCorpseIsSpawnedAtGround()
    {
        Vector3i position = positioner.FindSpawnLocationStartingFrom(new Vector3i(50, GROUND_HEIGHT + 50, 50));

        Assert.AreEqual(new Vector3i(50, GROUND_HEIGHT + 1, 50), position);
    }

    [Test]
    public void WhenCurrentPositionContainsCorpseThenCorpseIsSpawnedInAdjacentPosition()
    {
        int height = GROUND_HEIGHT + 1;

        Vector3i startingPosition = new Vector3i(50, height, 50);
        SetBlockAt(startingPosition, GenerateGoreBlock());

        Vector3i position = positioner.FindSpawnLocationStartingFrom(startingPosition);

        int deltaX = Math.Abs(position.x - 50);
        int deltaZ = Math.Abs(position.x - 50);
        Assert.LessOrEqual(deltaX, 1);
        Assert.LessOrEqual(deltaZ, 1);
        Assert.Greater(deltaX + deltaZ, 0);
        Assert.AreEqual(height, position.y);
    }

    [Test]
    public void WhenCurrentPositionContainsCorpseAndAdjacentPositionsAreGroundThenCorpseIsSpawnedInAdjacentPositionAboveGround()
    {
        int height = GROUND_HEIGHT;

        Vector3i startingPosition = new Vector3i(50, height, 50);
        SetBlockAt(startingPosition, GenerateGoreBlock());

        Vector3i position = positioner.FindSpawnLocationStartingFrom(startingPosition);

        int deltaX = Math.Abs(position.x - 50);
        int deltaZ = Math.Abs(position.x - 50);
        Assert.LessOrEqual(deltaX, 1);
        Assert.LessOrEqual(deltaZ, 1);
        Assert.Greater(deltaX + deltaZ, 0);
        Assert.AreEqual(height + 1, position.y);
    }

    [Test]
    public void WhenCurrentPositionContainsCorpseAndAdjacentPositionsAreAirThenCorpseIsSpawnedInAdjacentPositionAtGround()
    {
        int height = GROUND_HEIGHT + 2;

        Vector3i startingPosition = new Vector3i(50, height, 50);
        SetBlockAt(startingPosition, GenerateGoreBlock());
        SetBlockAt(new Vector3i(startingPosition.x,startingPosition.y-1,startingPosition.z), GenerateOccupiedBlock());

        Vector3i position = positioner.FindSpawnLocationStartingFrom(startingPosition);

        int deltaX = Math.Abs(position.x - 50);
        int deltaZ = Math.Abs(position.x - 50);
        Assert.LessOrEqual(deltaX, 1);
        Assert.LessOrEqual(deltaZ, 1);
        Assert.Greater(deltaX + deltaZ, 0);
        Assert.AreEqual(height - 1, position.y);
    }
    /*
    [Test]
    public void WhenCurrentPositionIsInBetweenTwoFloorsThenCorpseIsSpawnedAtCurrentPosition()
    {
        int height = GROUND_HEIGHT + 1;

        Vector3i startingPosition = new Vector3i(50, height, 50);
        SetBlockAt(new Vector3i(startingPosition.x, startingPosition.y - 2, startingPosition.z), GenerateEmptyBlock());
        SetBlockAt(new Vector3i(startingPosition.x, startingPosition.y + 1, startingPosition.z), GenerateOccupiedBlock());

        Vector3i position = positioner.FindSpawnLocationStartingFrom(startingPosition);

        Assert.AreEqual(new Vector3i(50, height, 50), position);
    }
    */
    private IBlock GetBlockAt(Vector3i position)
    {
        return fakeWorld[position.x, position.y, position.z];
    }

    private void SetBlockAt(Vector3i position, IBlock block)
    {
        fakeWorld[position.x, position.y, position.z] = block;
    }

    private IBlock GenerateOccupiedBlock()
    {
        return new FakeBlock(BlockTags.None, true);
    }

    private IBlock GenerateGoreBlock()
    {
        return new FakeBlock(BlockTags.Gore, false);
    }

    private IBlock GenerateEmptyBlock()
    {
        return new FakeBlock(BlockTags.None, false);
    }

    private class FakeBlock : IBlock
    {
        private BlockTags tag;
        private bool isCollideMovement;

        public FakeBlock(BlockTags tag, bool isCollideMovement)
        {
            this.tag = tag;
            this.isCollideMovement = isCollideMovement;
        }

        public BlockTags BlockTag => tag;

        public bool IsCollideMovement => isCollideMovement;
    }
}
