using System;
using System.Text;
using Unity.AI.Planner;
using Unity.AI.Planner.DomainLanguage.TraitBased;
using Unity.AI.Planner.Jobs;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine.AI.Planner.DomainLanguage.TraitBased;

namespace AI.Planner.Domains
{
    // Domains don't share key types to enforce that planners are domain specific
    public struct StateEntityKey : IEquatable<StateEntityKey>, IStateKey
    {
        public Entity Entity;
        public int HashCode;

        public bool Equals(StateEntityKey other) => Entity == other.Entity;

        public override int GetHashCode() => HashCode;

        public override string ToString() => $"StateEntityKey ({Entity} {HashCode})";
        public string Label => Entity.ToString();
    }

    public struct TerminationEvaluator : ITerminationEvaluator<StateData>
    {
        public bool IsTerminal(StateData state)
        {
            return
                new Die().IsTerminal(state) ||
            false;
        }
    }

    public struct Heuristic : IHeuristic<StateData>
    {
        public float Evaluate(StateData state)
        {
            return 0f;
        }
    }

    public static class TraitArrayIndex<T> where T : struct, ITrait
    {
        public static int Index = -1;
    }

    public struct DomainObject : IDomainObject
    {
        static DomainObject()
        {
            TraitArrayIndex<Agent>.Index = 0;
            TraitArrayIndex<Bed>.Index = 1;
            TraitArrayIndex<ConsumableDispenser>.Index = 2;
            TraitArrayIndex<Infrastructure>.Index = 3;
            TraitArrayIndex<Inventory>.Index = 4;
            TraitArrayIndex<Need>.Index = 5;
            TraitArrayIndex<ResourcesSpot>.Index = 6;
            TraitArrayIndex<Time>.Index = 7;
            TraitArrayIndex<WareHouse>.Index = 8;
            TraitArrayIndex<Location>.Index = 9;
        }

        public int Length => 10;

        public byte this[int i]
        {
            get
            {
                switch (i)
                {
                    case 0:
                        return AgentIndex;
                    case 1:
                        return BedIndex;
                    case 2:
                        return ConsumableDispenserIndex;
                    case 3:
                        return InfrastructureIndex;
                    case 4:
                        return InventoryIndex;
                    case 5:
                        return NeedIndex;
                    case 6:
                        return ResourcesSpotIndex;
                    case 7:
                        return TimeIndex;
                    case 8:
                        return WareHouseIndex;
                    case 9:
                        return LocationIndex;
                }

                return Unset;
            }
            set
            {
                switch (i)
                {
                    case 0:
                        AgentIndex = value;
                        break;
                    case 1:
                        BedIndex = value;
                        break;
                    case 2:
                        ConsumableDispenserIndex = value;
                        break;
                    case 3:
                        InfrastructureIndex = value;
                        break;
                    case 4:
                        InventoryIndex = value;
                        break;
                    case 5:
                        NeedIndex = value;
                        break;
                    case 6:
                        ResourcesSpotIndex = value;
                        break;
                    case 7:
                        TimeIndex = value;
                        break;
                    case 8:
                        WareHouseIndex = value;
                        break;
                    case 9:
                        LocationIndex = value;
                        break;
                }
            }
        }

        public static byte Unset = Byte.MaxValue;

        public static DomainObject Default => new DomainObject
        {
            AgentIndex = Unset,
            BedIndex = Unset,
            ConsumableDispenserIndex = Unset,
            InfrastructureIndex = Unset,
            InventoryIndex = Unset,
            NeedIndex = Unset,
            ResourcesSpotIndex = Unset,
            TimeIndex = Unset,
            WareHouseIndex = Unset,
            LocationIndex = Unset,
        };


        public byte AgentIndex;
        public byte BedIndex;
        public byte ConsumableDispenserIndex;
        public byte InfrastructureIndex;
        public byte InventoryIndex;
        public byte NeedIndex;
        public byte ResourcesSpotIndex;
        public byte TimeIndex;
        public byte WareHouseIndex;
        public byte LocationIndex;


        static ComponentType s_AgentType = new ComponentType(typeof(Agent));
        static ComponentType s_BedType = new ComponentType(typeof(Bed));
        static ComponentType s_ConsumableDispenserType = new ComponentType(typeof(ConsumableDispenser));
        static ComponentType s_InfrastructureType = new ComponentType(typeof(Infrastructure));
        static ComponentType s_InventoryType = new ComponentType(typeof(Inventory));
        static ComponentType s_NeedType = new ComponentType(typeof(Need));
        static ComponentType s_ResourcesSpotType = new ComponentType(typeof(ResourcesSpot));
        static ComponentType s_TimeType = new ComponentType(typeof(Time));
        static ComponentType s_WareHouseType = new ComponentType(typeof(WareHouse));
        static ComponentType s_LocationType = new ComponentType(typeof(Location));

        public bool HasSameTraits(DomainObject other)
        {
            for (var i = 0; i < Length; i++)
            {
                var traitIndex = this[i];
                var otherTraitIndex = other[i];
                if (traitIndex == Unset && otherTraitIndex != Unset || traitIndex != Unset && otherTraitIndex == Unset)
                    return false;
            }
            return true;
        }

        public bool HasTraitSubset(DomainObject traitSubset)
        {
            for (var i = 0; i < Length; i++)
            {
                var requiredTrait = traitSubset[i];
                if (requiredTrait != Unset && this[i] == Unset)
                    return false;
            }
            return true;
        }

        // todo - replace with more efficient subset check
        public bool MatchesTraitFilter(ComponentType[] componentTypes)
        {
            foreach (var t in componentTypes)
            {

                if (t == s_AgentType)
                {
                    if ((t.AccessModeType == ComponentType.AccessMode.Exclude && AgentIndex != Unset) ||
                        (t.AccessModeType != ComponentType.AccessMode.Exclude && AgentIndex == Unset))
                        return false;
                }
                else if (t == s_BedType)
                {
                    if ((t.AccessModeType == ComponentType.AccessMode.Exclude && BedIndex != Unset) ||
                        (t.AccessModeType != ComponentType.AccessMode.Exclude && BedIndex == Unset))
                        return false;
                }
                else if (t == s_ConsumableDispenserType)
                {
                    if ((t.AccessModeType == ComponentType.AccessMode.Exclude && ConsumableDispenserIndex != Unset) ||
                        (t.AccessModeType != ComponentType.AccessMode.Exclude && ConsumableDispenserIndex == Unset))
                        return false;
                }
                else if (t == s_InfrastructureType)
                {
                    if ((t.AccessModeType == ComponentType.AccessMode.Exclude && InfrastructureIndex != Unset) ||
                        (t.AccessModeType != ComponentType.AccessMode.Exclude && InfrastructureIndex == Unset))
                        return false;
                }
                else if (t == s_InventoryType)
                {
                    if ((t.AccessModeType == ComponentType.AccessMode.Exclude && InventoryIndex != Unset) ||
                        (t.AccessModeType != ComponentType.AccessMode.Exclude && InventoryIndex == Unset))
                        return false;
                }
                else if (t == s_NeedType)
                {
                    if ((t.AccessModeType == ComponentType.AccessMode.Exclude && NeedIndex != Unset) ||
                        (t.AccessModeType != ComponentType.AccessMode.Exclude && NeedIndex == Unset))
                        return false;
                }
                else if (t == s_ResourcesSpotType)
                {
                    if ((t.AccessModeType == ComponentType.AccessMode.Exclude && ResourcesSpotIndex != Unset) ||
                        (t.AccessModeType != ComponentType.AccessMode.Exclude && ResourcesSpotIndex == Unset))
                        return false;
                }
                else if (t == s_TimeType)
                {
                    if ((t.AccessModeType == ComponentType.AccessMode.Exclude && TimeIndex != Unset) ||
                        (t.AccessModeType != ComponentType.AccessMode.Exclude && TimeIndex == Unset))
                        return false;
                }
                else if (t == s_WareHouseType)
                {
                    if ((t.AccessModeType == ComponentType.AccessMode.Exclude && WareHouseIndex != Unset) ||
                        (t.AccessModeType != ComponentType.AccessMode.Exclude && WareHouseIndex == Unset))
                        return false;
                }
                else if (t == s_LocationType)
                {
                    if ((t.AccessModeType == ComponentType.AccessMode.Exclude && LocationIndex != Unset) ||
                        (t.AccessModeType != ComponentType.AccessMode.Exclude && LocationIndex == Unset))
                        return false;
                }
                else
                {
                    throw new ArgumentException($"Incorrect trait type used in domain object query: {t}");
                }
            }

            return true;
        }
    }

    public struct StateData : ITraitBasedStateData<DomainObject>
    {
        public Entity StateEntity;
        public DynamicBuffer<DomainObject> DomainObjects;
        public DynamicBuffer<DomainObjectID> DomainObjectIDs;

        public DynamicBuffer<Agent> AgentBuffer;
        public DynamicBuffer<Bed> BedBuffer;
        public DynamicBuffer<ConsumableDispenser> ConsumableDispenserBuffer;
        public DynamicBuffer<Infrastructure> InfrastructureBuffer;
        public DynamicBuffer<Inventory> InventoryBuffer;
        public DynamicBuffer<Need> NeedBuffer;
        public DynamicBuffer<ResourcesSpot> ResourcesSpotBuffer;
        public DynamicBuffer<Time> TimeBuffer;
        public DynamicBuffer<WareHouse> WareHouseBuffer;
        public DynamicBuffer<Location> LocationBuffer;

        static ComponentType s_AgentType = new ComponentType(typeof(Agent));
        static ComponentType s_BedType = new ComponentType(typeof(Bed));
        static ComponentType s_ConsumableDispenserType = new ComponentType(typeof(ConsumableDispenser));
        static ComponentType s_InfrastructureType = new ComponentType(typeof(Infrastructure));
        static ComponentType s_InventoryType = new ComponentType(typeof(Inventory));
        static ComponentType s_NeedType = new ComponentType(typeof(Need));
        static ComponentType s_ResourcesSpotType = new ComponentType(typeof(ResourcesSpot));
        static ComponentType s_TimeType = new ComponentType(typeof(Time));
        static ComponentType s_WareHouseType = new ComponentType(typeof(WareHouse));
        static ComponentType s_LocationType = new ComponentType(typeof(Location));

        public StateData(JobComponentSystem system, Entity stateEntity, bool readWrite = false)
        {
            StateEntity = stateEntity;
            DomainObjects = system.GetBufferFromEntity<DomainObject>(!readWrite)[stateEntity];
            DomainObjectIDs = system.GetBufferFromEntity<DomainObjectID>(!readWrite)[stateEntity];

            AgentBuffer = system.GetBufferFromEntity<Agent>(!readWrite)[stateEntity];
            BedBuffer = system.GetBufferFromEntity<Bed>(!readWrite)[stateEntity];
            ConsumableDispenserBuffer = system.GetBufferFromEntity<ConsumableDispenser>(!readWrite)[stateEntity];
            InfrastructureBuffer = system.GetBufferFromEntity<Infrastructure>(!readWrite)[stateEntity];
            InventoryBuffer = system.GetBufferFromEntity<Inventory>(!readWrite)[stateEntity];
            NeedBuffer = system.GetBufferFromEntity<Need>(!readWrite)[stateEntity];
            ResourcesSpotBuffer = system.GetBufferFromEntity<ResourcesSpot>(!readWrite)[stateEntity];
            TimeBuffer = system.GetBufferFromEntity<Time>(!readWrite)[stateEntity];
            WareHouseBuffer = system.GetBufferFromEntity<WareHouse>(!readWrite)[stateEntity];
            LocationBuffer = system.GetBufferFromEntity<Location>(!readWrite)[stateEntity];
        }

        public StateData(int jobIndex, EntityCommandBuffer.Concurrent entityCommandBuffer, Entity stateEntity)
        {
            StateEntity = stateEntity;
            DomainObjects = entityCommandBuffer.AddBuffer<DomainObject>(jobIndex, stateEntity);
            DomainObjectIDs = entityCommandBuffer.AddBuffer<DomainObjectID>(jobIndex, stateEntity);

            AgentBuffer = entityCommandBuffer.AddBuffer<Agent>(jobIndex, stateEntity);
            BedBuffer = entityCommandBuffer.AddBuffer<Bed>(jobIndex, stateEntity);
            ConsumableDispenserBuffer = entityCommandBuffer.AddBuffer<ConsumableDispenser>(jobIndex, stateEntity);
            InfrastructureBuffer = entityCommandBuffer.AddBuffer<Infrastructure>(jobIndex, stateEntity);
            InventoryBuffer = entityCommandBuffer.AddBuffer<Inventory>(jobIndex, stateEntity);
            NeedBuffer = entityCommandBuffer.AddBuffer<Need>(jobIndex, stateEntity);
            ResourcesSpotBuffer = entityCommandBuffer.AddBuffer<ResourcesSpot>(jobIndex, stateEntity);
            TimeBuffer = entityCommandBuffer.AddBuffer<Time>(jobIndex, stateEntity);
            WareHouseBuffer = entityCommandBuffer.AddBuffer<WareHouse>(jobIndex, stateEntity);
            LocationBuffer = entityCommandBuffer.AddBuffer<Location>(jobIndex, stateEntity);
        }

        public StateData Copy(int jobIndex, EntityCommandBuffer.Concurrent entityCommandBuffer)
        {
            var stateEntity = entityCommandBuffer.Instantiate(jobIndex, StateEntity);
            var domainObjects = entityCommandBuffer.SetBuffer<DomainObject>(jobIndex, stateEntity);
            domainObjects.CopyFrom(DomainObjects.AsNativeArray());
            var domainObjectIDs = entityCommandBuffer.SetBuffer<DomainObjectID>(jobIndex, stateEntity);
            domainObjectIDs.CopyFrom(DomainObjectIDs.AsNativeArray());

            var Agents = entityCommandBuffer.SetBuffer<Agent>(jobIndex, stateEntity);
            Agents.CopyFrom(AgentBuffer.AsNativeArray());
            var Beds = entityCommandBuffer.SetBuffer<Bed>(jobIndex, stateEntity);
            Beds.CopyFrom(BedBuffer.AsNativeArray());
            var ConsumableDispensers = entityCommandBuffer.SetBuffer<ConsumableDispenser>(jobIndex, stateEntity);
            ConsumableDispensers.CopyFrom(ConsumableDispenserBuffer.AsNativeArray());
            var Infrastructures = entityCommandBuffer.SetBuffer<Infrastructure>(jobIndex, stateEntity);
            Infrastructures.CopyFrom(InfrastructureBuffer.AsNativeArray());
            var Inventorys = entityCommandBuffer.SetBuffer<Inventory>(jobIndex, stateEntity);
            Inventorys.CopyFrom(InventoryBuffer.AsNativeArray());
            var Needs = entityCommandBuffer.SetBuffer<Need>(jobIndex, stateEntity);
            Needs.CopyFrom(NeedBuffer.AsNativeArray());
            var ResourcesSpots = entityCommandBuffer.SetBuffer<ResourcesSpot>(jobIndex, stateEntity);
            ResourcesSpots.CopyFrom(ResourcesSpotBuffer.AsNativeArray());
            var Times = entityCommandBuffer.SetBuffer<Time>(jobIndex, stateEntity);
            Times.CopyFrom(TimeBuffer.AsNativeArray());
            var WareHouses = entityCommandBuffer.SetBuffer<WareHouse>(jobIndex, stateEntity);
            WareHouses.CopyFrom(WareHouseBuffer.AsNativeArray());
            var Locations = entityCommandBuffer.SetBuffer<Location>(jobIndex, stateEntity);
            Locations.CopyFrom(LocationBuffer.AsNativeArray());

            return new StateData
            {
                StateEntity = stateEntity,
                DomainObjects = domainObjects,
                DomainObjectIDs = domainObjectIDs,

                AgentBuffer = Agents,
                BedBuffer = Beds,
                ConsumableDispenserBuffer = ConsumableDispensers,
                InfrastructureBuffer = Infrastructures,
                InventoryBuffer = Inventorys,
                NeedBuffer = Needs,
                ResourcesSpotBuffer = ResourcesSpots,
                TimeBuffer = Times,
                WareHouseBuffer = WareHouses,
                LocationBuffer = Locations,
            };
        }

        public (DomainObject, DomainObjectID) AddDomainObject(ComponentType[] types, string name = null)
        {
            var domainObject = DomainObject.Default;
            var domainObjectID = new DomainObjectID() { ID = ObjectID.GetNext() };
#if DEBUG
            if (!string.IsNullOrEmpty(name))
                domainObjectID.Name.CopyFrom(name);
#endif

            foreach (var t in types)
            {
                if (t == s_AgentType)
                {
                    AgentBuffer.Add(default);
                    domainObject.AgentIndex = (byte) (AgentBuffer.Length - 1);
                }
                else if (t == s_BedType)
                {
                    BedBuffer.Add(default);
                    domainObject.BedIndex = (byte) (BedBuffer.Length - 1);
                }
                else if (t == s_ConsumableDispenserType)
                {
                    ConsumableDispenserBuffer.Add(default);
                    domainObject.ConsumableDispenserIndex = (byte) (ConsumableDispenserBuffer.Length - 1);
                }
                else if (t == s_InfrastructureType)
                {
                    InfrastructureBuffer.Add(default);
                    domainObject.InfrastructureIndex = (byte) (InfrastructureBuffer.Length - 1);
                }
                else if (t == s_InventoryType)
                {
                    InventoryBuffer.Add(default);
                    domainObject.InventoryIndex = (byte) (InventoryBuffer.Length - 1);
                }
                else if (t == s_NeedType)
                {
                    NeedBuffer.Add(default);
                    domainObject.NeedIndex = (byte) (NeedBuffer.Length - 1);
                }
                else if (t == s_ResourcesSpotType)
                {
                    ResourcesSpotBuffer.Add(default);
                    domainObject.ResourcesSpotIndex = (byte) (ResourcesSpotBuffer.Length - 1);
                }
                else if (t == s_TimeType)
                {
                    TimeBuffer.Add(default);
                    domainObject.TimeIndex = (byte) (TimeBuffer.Length - 1);
                }
                else if (t == s_WareHouseType)
                {
                    WareHouseBuffer.Add(default);
                    domainObject.WareHouseIndex = (byte) (WareHouseBuffer.Length - 1);
                }
                else if (t == s_LocationType)
                {
                    LocationBuffer.Add(default);
                    domainObject.LocationIndex = (byte) (LocationBuffer.Length - 1);
                }
            }

            DomainObjectIDs.Add(domainObjectID);
            DomainObjects.Add(domainObject);

            return (domainObject, domainObjectID);
        }

        public void SetTraitOnObject(ITrait trait, ref DomainObject domainObject)
        {
            if (trait is Agent AgentTrait)
                SetTraitOnObject(AgentTrait, ref domainObject);
            else if (trait is Bed BedTrait)
                SetTraitOnObject(BedTrait, ref domainObject);
            else if (trait is ConsumableDispenser ConsumableDispenserTrait)
                SetTraitOnObject(ConsumableDispenserTrait, ref domainObject);
            else if (trait is Infrastructure InfrastructureTrait)
                SetTraitOnObject(InfrastructureTrait, ref domainObject);
            else if (trait is Inventory InventoryTrait)
                SetTraitOnObject(InventoryTrait, ref domainObject);
            else if (trait is Need NeedTrait)
                SetTraitOnObject(NeedTrait, ref domainObject);
            else if (trait is ResourcesSpot ResourcesSpotTrait)
                SetTraitOnObject(ResourcesSpotTrait, ref domainObject);
            else if (trait is Time TimeTrait)
                SetTraitOnObject(TimeTrait, ref domainObject);
            else if (trait is WareHouse WareHouseTrait)
                SetTraitOnObject(WareHouseTrait, ref domainObject);
            else if (trait is Location LocationTrait)
                SetTraitOnObject(LocationTrait, ref domainObject);
            else 
                throw new ArgumentException($"Trait {trait} of type {trait.GetType()} is not supported in this domain.");
        }


        public TTrait GetTraitOnObject<TTrait>(DomainObject domainObject) where TTrait : struct, ITrait
        {
            var domainObjectTraitIndex = TraitArrayIndex<TTrait>.Index;
            if (domainObjectTraitIndex == -1)
                throw new ArgumentException($"Trait {typeof(TTrait)} not supported in this domain");

            var traitBufferIndex = domainObject[domainObjectTraitIndex];
            if (traitBufferIndex == DomainObject.Unset)
                throw new ArgumentException($"Trait of type {typeof(TTrait)} does not exist on object {domainObject}.");

            return GetBuffer<TTrait>()[traitBufferIndex];
        }

        public void SetTraitOnObject<TTrait>(TTrait trait, ref DomainObject domainObject) where TTrait : struct, ITrait
        {
            var objectIndex = GetDomainObjectIndex(domainObject);
            if (objectIndex == -1)
                throw new ArgumentException($"Object {domainObject} does not exist within the state data {this}.");

            var traitIndex = TraitArrayIndex<TTrait>.Index;
            var traitBuffer = GetBuffer<TTrait>();

            var bufferIndex = domainObject[traitIndex];
            if (bufferIndex == DomainObject.Unset)
            {
                traitBuffer.Add(trait);
                domainObject[traitIndex] = (byte) (traitBuffer.Length - 1);

                DomainObjects[objectIndex] = domainObject;
            }
            else
            {
                traitBuffer[bufferIndex] = trait;
            }
        }

        public bool RemoveTraitOnObject<TTrait>(ref DomainObject domainObject) where TTrait : struct, ITrait
        {
            var objectTraitIndex = TraitArrayIndex<TTrait>.Index;
            var traitBuffer = GetBuffer<TTrait>();

            var traitBufferIndex = domainObject[objectTraitIndex];
            if (traitBufferIndex == DomainObject.Unset)
                return false;

            // last index
            var lastBufferIndex = traitBuffer.Length - 1;

            // Swap back and remove
            var lastTrait = traitBuffer[lastBufferIndex];
            traitBuffer[lastBufferIndex] = traitBuffer[traitBufferIndex];
            traitBuffer[traitBufferIndex] = lastTrait;
            traitBuffer.RemoveAt(lastBufferIndex);

            // Update index for object with last trait in buffer
            for (int i = 0; i < DomainObjects.Length; i++)
            {
                var otherDomainObject = DomainObjects[i];
                if (otherDomainObject[objectTraitIndex] == lastBufferIndex)
                {
                    otherDomainObject[objectTraitIndex] = traitBufferIndex;
                    DomainObjects[i] = otherDomainObject;
                    break;
                }
            }

            // Update domainObject in buffer (ref is to a copy)
            for (int i = 0; i < DomainObjects.Length; i++)
            {
                if (domainObject.Equals(DomainObjects[i]))
                {
                    domainObject[objectTraitIndex] = DomainObject.Unset;
                    DomainObjects[i] = domainObject;
                    return true;
                }
            }

            throw new ArgumentException($"DomainObject {domainObject} does not exist in the state container {this}.");
        }

        public bool RemoveDomainObject(DomainObject domainObject)
        {
            var objectIndex = GetDomainObjectIndex(domainObject);
            if (objectIndex == -1)
                return false;


            RemoveTraitOnObject<Agent>(ref domainObject);
            RemoveTraitOnObject<Bed>(ref domainObject);
            RemoveTraitOnObject<ConsumableDispenser>(ref domainObject);
            RemoveTraitOnObject<Infrastructure>(ref domainObject);
            RemoveTraitOnObject<Inventory>(ref domainObject);
            RemoveTraitOnObject<Need>(ref domainObject);
            RemoveTraitOnObject<ResourcesSpot>(ref domainObject);
            RemoveTraitOnObject<Time>(ref domainObject);
            RemoveTraitOnObject<WareHouse>(ref domainObject);
            RemoveTraitOnObject<Location>(ref domainObject);

            DomainObjects.RemoveAt(objectIndex);
            DomainObjectIDs.RemoveAt(objectIndex);

            return true;
        }


        public TTrait GetTraitOnObjectAtIndex<TTrait>(int domainObjectIndex) where TTrait : struct, ITrait
        {
            var domainObjectTraitIndex = TraitArrayIndex<TTrait>.Index;
            if (domainObjectTraitIndex == -1)
                throw new ArgumentException($"Trait {typeof(TTrait)} not supported in this domain");

            var domainObject = DomainObjects[domainObjectIndex];
            var traitBufferIndex = domainObject[domainObjectTraitIndex];
            if (traitBufferIndex == DomainObject.Unset)
                throw new Exception($"Trait index for {typeof(TTrait)} is not set for domain object {domainObject}");

            return GetBuffer<TTrait>()[traitBufferIndex];
        }

        public void SetTraitOnObjectAtIndex<T>(T trait, int domainObjectIndex) where T : struct, ITrait
        {
            var domainObjectTraitIndex = TraitArrayIndex<T>.Index;
            if (domainObjectTraitIndex == -1)
                throw new ArgumentException($"Trait {typeof(T)} not supported in this domain");

            var domainObject = DomainObjects[domainObjectIndex];
            var traitBufferIndex = domainObject[domainObjectTraitIndex];
            var traitBuffer = GetBuffer<T>();
            if (traitBufferIndex == DomainObject.Unset)
            {
                traitBuffer.Add(trait);
                traitBufferIndex = (byte)(traitBuffer.Length - 1);
                domainObject[domainObjectTraitIndex] = traitBufferIndex;
                DomainObjects[domainObjectIndex] = domainObject;
            }
            else
            {
                traitBuffer[traitBufferIndex] = trait;
            }
        }

        public bool RemoveTraitOnObjectAtIndex<TTrait>(int domainObjectIndex) where TTrait : struct, ITrait
        {
            var objectTraitIndex = TraitArrayIndex<TTrait>.Index;
            var traitBuffer = GetBuffer<TTrait>();

            var domainObject = DomainObjects[domainObjectIndex];
            var traitBufferIndex = domainObject[objectTraitIndex];
            if (traitBufferIndex == DomainObject.Unset)
                return false;

            // last index
            var lastBufferIndex = traitBuffer.Length - 1;

            // Swap back and remove
            var lastTrait = traitBuffer[lastBufferIndex];
            traitBuffer[lastBufferIndex] = traitBuffer[traitBufferIndex];
            traitBuffer[traitBufferIndex] = lastTrait;
            traitBuffer.RemoveAt(lastBufferIndex);

            // Update index for object with last trait in buffer
            for (int i = 0; i < DomainObjects.Length; i++)
            {
                var otherDomainObject = DomainObjects[i];
                if (otherDomainObject[objectTraitIndex] == lastBufferIndex)
                {
                    otherDomainObject[objectTraitIndex] = traitBufferIndex;
                    DomainObjects[i] = otherDomainObject;
                    break;
                }
            }

            domainObject[objectTraitIndex] = DomainObject.Unset;
            DomainObjects[domainObjectIndex] = domainObject;

            throw new ArgumentException($"DomainObject {domainObject} does not exist in the state container {this}.");
        }

        public bool RemoveDomainObjectAtIndex(int domainObjectIndex)
        {
            RemoveTraitOnObjectAtIndex<Agent>(domainObjectIndex);
            RemoveTraitOnObjectAtIndex<Bed>(domainObjectIndex);
            RemoveTraitOnObjectAtIndex<ConsumableDispenser>(domainObjectIndex);
            RemoveTraitOnObjectAtIndex<Infrastructure>(domainObjectIndex);
            RemoveTraitOnObjectAtIndex<Inventory>(domainObjectIndex);
            RemoveTraitOnObjectAtIndex<Need>(domainObjectIndex);
            RemoveTraitOnObjectAtIndex<ResourcesSpot>(domainObjectIndex);
            RemoveTraitOnObjectAtIndex<Time>(domainObjectIndex);
            RemoveTraitOnObjectAtIndex<WareHouse>(domainObjectIndex);
            RemoveTraitOnObjectAtIndex<Location>(domainObjectIndex);

            DomainObjects.RemoveAt(domainObjectIndex);
            DomainObjectIDs.RemoveAt(domainObjectIndex);

            return true;
        }


        public NativeArray<(DomainObject, int)> GetDomainObjects(NativeList<(DomainObject, int)> domainObjects, params ComponentType[] traitFilter)
        {
            for (var i = 0; i < DomainObjects.Length; i++)
            {
                var domainObject = DomainObjects[i];
                if (domainObject.MatchesTraitFilter(traitFilter))
                    domainObjects.Add((domainObject, i));
            }

            return domainObjects.AsArray();
        }

        public void GetDomainObjectIndices(DomainObject traitSubset, NativeList<int> domainObjects)
        {
            var domainObjectArray = DomainObjects.AsNativeArray();
            for (var i = 0; i < domainObjectArray.Length; i++)
            {
                if (domainObjectArray[i].HasTraitSubset(traitSubset))
                    domainObjects.Add(i);
            }
        }

        public int GetDomainObjectIndex(DomainObject domainObject)
        {
            var objectIndex = -1;
            for (int i = 0; i < DomainObjects.Length; i++)
            {
                if (DomainObjects[i].Equals(domainObject))
                {
                    objectIndex = i;
                    break;
                }
            }

            return objectIndex;
        }

        public int GetDomainObjectIndex(DomainObjectID domainObjectId)
        {
            var objectIndex = -1;
            for (int i = 0; i < DomainObjectIDs.Length; i++)
            {
                if (DomainObjectIDs[i].Equals(domainObjectId))
                {
                    objectIndex = i;
                    break;
                }
            }

            return objectIndex;
        }

        public DomainObjectID GetDomainObjectID(DomainObject domainObject)
        {
            var index = GetDomainObjectIndex(domainObject);
            return DomainObjectIDs[index];
        }

        public DomainObjectID GetDomainObjectID(int domainObjectIndex)
        {
            return DomainObjectIDs[domainObjectIndex];
        }

        public DomainObject GetDomainObject(DomainObjectID domainObject)
        {
            var index = GetDomainObjectIndex(domainObject);
            return DomainObjects[index];
        }


        DynamicBuffer<T> GetBuffer<T>() where T : struct, ITrait
        {
            var index = TraitArrayIndex<T>.Index;
            switch (index)
            {
                case 0:
                    return AgentBuffer.Reinterpret<T>();
                case 1:
                    return BedBuffer.Reinterpret<T>();
                case 2:
                    return ConsumableDispenserBuffer.Reinterpret<T>();
                case 3:
                    return InfrastructureBuffer.Reinterpret<T>();
                case 4:
                    return InventoryBuffer.Reinterpret<T>();
                case 5:
                    return NeedBuffer.Reinterpret<T>();
                case 6:
                    return ResourcesSpotBuffer.Reinterpret<T>();
                case 7:
                    return TimeBuffer.Reinterpret<T>();
                case 8:
                    return WareHouseBuffer.Reinterpret<T>();
                case 9:
                    return LocationBuffer.Reinterpret<T>();
            }

            return default;
        }

        public bool Equals(StateData other)
        {
            if (StateEntity == other.StateEntity)
                return true;

            // Easy check is to make sure each state has the same number of domain objects
            if (DomainObjects.Length != other.DomainObjects.Length
                || AgentBuffer.Length != other.AgentBuffer.Length
                || BedBuffer.Length != other.BedBuffer.Length
                || ConsumableDispenserBuffer.Length != other.ConsumableDispenserBuffer.Length
                || InfrastructureBuffer.Length != other.InfrastructureBuffer.Length
                || InventoryBuffer.Length != other.InventoryBuffer.Length
                || NeedBuffer.Length != other.NeedBuffer.Length
                || ResourcesSpotBuffer.Length != other.ResourcesSpotBuffer.Length
                || TimeBuffer.Length != other.TimeBuffer.Length
                || WareHouseBuffer.Length != other.WareHouseBuffer.Length
                || LocationBuffer.Length != other.LocationBuffer.Length)
                return false;

            var lhsObjects = DomainObjects.AsNativeArray();
            var rhsObjects = new NativeArray<DomainObject>(other.DomainObjects.Length, Allocator.Temp);
            rhsObjects.CopyFrom(other.DomainObjects.AsNativeArray());

            for (int lhsIndex = 0; lhsIndex < lhsObjects.Length; lhsIndex++)
            {
                var domainObjectLHS = lhsObjects[lhsIndex];

                var hasAgent = domainObjectLHS.AgentIndex != DomainObject.Unset;
                var hasBed = domainObjectLHS.BedIndex != DomainObject.Unset;
                var hasConsumableDispenser = domainObjectLHS.ConsumableDispenserIndex != DomainObject.Unset;
                var hasInfrastructure = domainObjectLHS.InfrastructureIndex != DomainObject.Unset;
                var hasInventory = domainObjectLHS.InventoryIndex != DomainObject.Unset;
                var hasNeed = domainObjectLHS.NeedIndex != DomainObject.Unset;
                var hasResourcesSpot = domainObjectLHS.ResourcesSpotIndex != DomainObject.Unset;
                var hasTime = domainObjectLHS.TimeIndex != DomainObject.Unset;
                var hasWareHouse = domainObjectLHS.WareHouseIndex != DomainObject.Unset;
                var hasLocation = domainObjectLHS.LocationIndex != DomainObject.Unset;

                var foundMatch = false;
                for (var rhsIndex = lhsIndex; rhsIndex < rhsObjects.Length; rhsIndex++)
                {
                    var domainObjectRHS = rhsObjects[rhsIndex];

                    if (!domainObjectLHS.HasSameTraits(domainObjectRHS))
                        continue;

                    if (hasAgent && !AgentBuffer[domainObjectLHS.AgentIndex].Equals(other.AgentBuffer[domainObjectRHS.AgentIndex]))
                        continue;
                    if (hasBed && !BedBuffer[domainObjectLHS.BedIndex].Equals(other.BedBuffer[domainObjectRHS.BedIndex]))
                        continue;
                    if (hasConsumableDispenser && !ConsumableDispenserBuffer[domainObjectLHS.ConsumableDispenserIndex].Equals(other.ConsumableDispenserBuffer[domainObjectRHS.ConsumableDispenserIndex]))
                        continue;
                    if (hasInfrastructure && !InfrastructureBuffer[domainObjectLHS.InfrastructureIndex].Equals(other.InfrastructureBuffer[domainObjectRHS.InfrastructureIndex]))
                        continue;
                    if (hasInventory && !InventoryBuffer[domainObjectLHS.InventoryIndex].Equals(other.InventoryBuffer[domainObjectRHS.InventoryIndex]))
                        continue;
                    if (hasNeed && !NeedBuffer[domainObjectLHS.NeedIndex].Equals(other.NeedBuffer[domainObjectRHS.NeedIndex]))
                        continue;
                    if (hasResourcesSpot && !ResourcesSpotBuffer[domainObjectLHS.ResourcesSpotIndex].Equals(other.ResourcesSpotBuffer[domainObjectRHS.ResourcesSpotIndex]))
                        continue;
                    if (hasTime && !TimeBuffer[domainObjectLHS.TimeIndex].Equals(other.TimeBuffer[domainObjectRHS.TimeIndex]))
                        continue;
                    if (hasWareHouse && !WareHouseBuffer[domainObjectLHS.WareHouseIndex].Equals(other.WareHouseBuffer[domainObjectRHS.WareHouseIndex]))
                        continue;
                    if (hasLocation && !LocationBuffer[domainObjectLHS.LocationIndex].Equals(other.LocationBuffer[domainObjectRHS.LocationIndex]))
                        continue;

                    // Swap match to lhs index (to align). Keeps remaining objs in latter part of array without resize.
                    rhsObjects[rhsIndex] = rhsObjects[lhsIndex];  // technically only Lockable to preserve obj at lhsIndex

                    foundMatch = true;
                    break;
                }

                if (!foundMatch)
                {
                    rhsObjects.Dispose();
                    return false;
                }
            }

            rhsObjects.Dispose();
            return true;
        }

        public override int GetHashCode()
        {
            // h = 3860031 + (h+y)*2779 + (h*y*2)   // from How to Hash a Set by Richard O’Keefe
            var stateHashValue = 0;

            var objectIDs = DomainObjectIDs;
            foreach (var element in objectIDs.AsNativeArray())
            {
                var value = element.GetHashCode();
                stateHashValue = 3860031 + (stateHashValue + value) * 2779 + (stateHashValue * value * 2);
            }

            foreach (var element in AgentBuffer.AsNativeArray())
            {
                var value = element.GetHashCode();
                stateHashValue = 3860031 + (stateHashValue + value) * 2779 + (stateHashValue * value * 2);
            }
            foreach (var element in BedBuffer.AsNativeArray())
            {
                var value = element.GetHashCode();
                stateHashValue = 3860031 + (stateHashValue + value) * 2779 + (stateHashValue * value * 2);
            }
            foreach (var element in ConsumableDispenserBuffer.AsNativeArray())
            {
                var value = element.GetHashCode();
                stateHashValue = 3860031 + (stateHashValue + value) * 2779 + (stateHashValue * value * 2);
            }
            foreach (var element in InfrastructureBuffer.AsNativeArray())
            {
                var value = element.GetHashCode();
                stateHashValue = 3860031 + (stateHashValue + value) * 2779 + (stateHashValue * value * 2);
            }
            foreach (var element in InventoryBuffer.AsNativeArray())
            {
                var value = element.GetHashCode();
                stateHashValue = 3860031 + (stateHashValue + value) * 2779 + (stateHashValue * value * 2);
            }
            foreach (var element in NeedBuffer.AsNativeArray())
            {
                var value = element.GetHashCode();
                stateHashValue = 3860031 + (stateHashValue + value) * 2779 + (stateHashValue * value * 2);
            }
            foreach (var element in ResourcesSpotBuffer.AsNativeArray())
            {
                var value = element.GetHashCode();
                stateHashValue = 3860031 + (stateHashValue + value) * 2779 + (stateHashValue * value * 2);
            }
            foreach (var element in TimeBuffer.AsNativeArray())
            {
                var value = element.GetHashCode();
                stateHashValue = 3860031 + (stateHashValue + value) * 2779 + (stateHashValue * value * 2);
            }
            foreach (var element in WareHouseBuffer.AsNativeArray())
            {
                var value = element.GetHashCode();
                stateHashValue = 3860031 + (stateHashValue + value) * 2779 + (stateHashValue * value * 2);
            }
            foreach (var element in LocationBuffer.AsNativeArray())
            {
                var value = element.GetHashCode();
                stateHashValue = 3860031 + (stateHashValue + value) * 2779 + (stateHashValue * value * 2);
            }

            return stateHashValue;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            for (var domainObjectIndex = 0; domainObjectIndex < DomainObjects.Length; domainObjectIndex++)
            {
                var domainObject = DomainObjects[domainObjectIndex];
                sb.AppendLine(DomainObjectIDs[domainObjectIndex].ToString());

                var i = 0;

                var traitIndex = domainObject[i++];
                if (traitIndex != DomainObject.Unset)
                    sb.AppendLine(AgentBuffer[traitIndex].ToString());

                traitIndex = domainObject[i++];
                if (traitIndex != DomainObject.Unset)
                    sb.AppendLine(BedBuffer[traitIndex].ToString());

                traitIndex = domainObject[i++];
                if (traitIndex != DomainObject.Unset)
                    sb.AppendLine(ConsumableDispenserBuffer[traitIndex].ToString());

                traitIndex = domainObject[i++];
                if (traitIndex != DomainObject.Unset)
                    sb.AppendLine(InfrastructureBuffer[traitIndex].ToString());

                traitIndex = domainObject[i++];
                if (traitIndex != DomainObject.Unset)
                    sb.AppendLine(InventoryBuffer[traitIndex].ToString());

                traitIndex = domainObject[i++];
                if (traitIndex != DomainObject.Unset)
                    sb.AppendLine(NeedBuffer[traitIndex].ToString());

                traitIndex = domainObject[i++];
                if (traitIndex != DomainObject.Unset)
                    sb.AppendLine(ResourcesSpotBuffer[traitIndex].ToString());

                traitIndex = domainObject[i++];
                if (traitIndex != DomainObject.Unset)
                    sb.AppendLine(TimeBuffer[traitIndex].ToString());

                traitIndex = domainObject[i++];
                if (traitIndex != DomainObject.Unset)
                    sb.AppendLine(WareHouseBuffer[traitIndex].ToString());

                traitIndex = domainObject[i++];
                if (traitIndex != DomainObject.Unset)
                    sb.AppendLine(LocationBuffer[traitIndex].ToString());

                sb.AppendLine();
            }

            return sb.ToString();
        }
    }

    public struct StateDataContext : ITraitBasedStateDataContext<DomainObject, StateEntityKey, StateData>
    {
        internal EntityCommandBuffer.Concurrent EntityCommandBuffer;
        internal EntityArchetype m_StateArchetype;
        internal int JobIndex; //todo assign

        [ReadOnly] public BufferFromEntity<DomainObject> DomainObjects;
        [ReadOnly] public BufferFromEntity<DomainObjectID> DomainObjectIDs;

        [ReadOnly] public BufferFromEntity<Agent> AgentData;
        [ReadOnly] public BufferFromEntity<Bed> BedData;
        [ReadOnly] public BufferFromEntity<ConsumableDispenser> ConsumableDispenserData;
        [ReadOnly] public BufferFromEntity<Infrastructure> InfrastructureData;
        [ReadOnly] public BufferFromEntity<Inventory> InventoryData;
        [ReadOnly] public BufferFromEntity<Need> NeedData;
        [ReadOnly] public BufferFromEntity<ResourcesSpot> ResourcesSpotData;
        [ReadOnly] public BufferFromEntity<Time> TimeData;
        [ReadOnly] public BufferFromEntity<WareHouse> WareHouseData;
        [ReadOnly] public BufferFromEntity<Location> LocationData;

        public StateDataContext(JobComponentSystem system, EntityArchetype stateArchetype)
        {
            EntityCommandBuffer = default;
            DomainObjects = system.GetBufferFromEntity<DomainObject>(true);
            DomainObjectIDs = system.GetBufferFromEntity<DomainObjectID>(true);

            AgentData = system.GetBufferFromEntity<Agent>(true);
            BedData = system.GetBufferFromEntity<Bed>(true);
            ConsumableDispenserData = system.GetBufferFromEntity<ConsumableDispenser>(true);
            InfrastructureData = system.GetBufferFromEntity<Infrastructure>(true);
            InventoryData = system.GetBufferFromEntity<Inventory>(true);
            NeedData = system.GetBufferFromEntity<Need>(true);
            ResourcesSpotData = system.GetBufferFromEntity<ResourcesSpot>(true);
            TimeData = system.GetBufferFromEntity<Time>(true);
            WareHouseData = system.GetBufferFromEntity<WareHouse>(true);
            LocationData = system.GetBufferFromEntity<Location>(true);

            m_StateArchetype = stateArchetype;
            JobIndex = 0; // todo set on all actions
        }

        public StateData GetStateData(StateEntityKey stateKey)
        {
            var stateEntity = stateKey.Entity;

            return new StateData
            {
                StateEntity = stateEntity,
                DomainObjects = DomainObjects[stateEntity],
                DomainObjectIDs = DomainObjectIDs[stateEntity],

                AgentBuffer = AgentData[stateEntity],
                BedBuffer = BedData[stateEntity],
                ConsumableDispenserBuffer = ConsumableDispenserData[stateEntity],
                InfrastructureBuffer = InfrastructureData[stateEntity],
                InventoryBuffer = InventoryData[stateEntity],
                NeedBuffer = NeedData[stateEntity],
                ResourcesSpotBuffer = ResourcesSpotData[stateEntity],
                TimeBuffer = TimeData[stateEntity],
                WareHouseBuffer = WareHouseData[stateEntity],
                LocationBuffer = LocationData[stateEntity],
            };
        }

        public StateData CopyStateData(StateData stateData)
        {
            return stateData.Copy(JobIndex, EntityCommandBuffer);
        }

        public StateEntityKey GetStateDataKey(StateData stateData)
        {
            return new StateEntityKey { Entity = stateData.StateEntity, HashCode = stateData.GetHashCode()};
        }

        public void DestroyState(StateEntityKey stateKey)
        {
            EntityCommandBuffer.DestroyEntity(JobIndex, stateKey.Entity);
        }

        public StateData CreateStateData()
        {
            return new StateData(JobIndex, EntityCommandBuffer, EntityCommandBuffer.CreateEntity(JobIndex, m_StateArchetype));
        }

        public bool Equals(StateData x, StateData y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(StateData obj)
        {
            return obj.GetHashCode();
        }
    }

    public class StateManager : JobComponentSystem, ITraitBasedStateManager<DomainObject, StateEntityKey, StateData, StateDataContext>, IStateManagerInternal
    {
        EntityArchetype m_StateArchetype;

        public StateManager()
        {
            m_StateArchetype = default;
        }

        protected override void OnCreate()
        {
            m_StateArchetype = EntityManager.CreateArchetype(typeof(State), typeof(DomainObject), typeof(DomainObjectID), typeof(HashCode),
                typeof(Agent),
                typeof(Bed),
                typeof(ConsumableDispenser),
                typeof(Infrastructure),
                typeof(Inventory),
                typeof(Need),
                typeof(ResourcesSpot),
                typeof(Time),
                typeof(WareHouse),
                typeof(Location));
        }

        public StateData CreateStateData()
        {
            var stateEntity = EntityManager.CreateEntity(m_StateArchetype);
            return new StateData(this, stateEntity, true);
        }

        public StateData GetStateData(StateEntityKey stateKey, bool readWrite = false)
        {
            return !Enabled ? default : new StateData(this, stateKey.Entity, readWrite);
        }

        public void DestroyState(StateEntityKey stateKey)
        {
            EntityManager?.DestroyEntity(stateKey.Entity);
        }

        public StateDataContext GetStateDataContext()
        {
            return new StateDataContext(this, m_StateArchetype);
        }

        public StateEntityKey GetStateDataKey(StateData stateData)
        {
            return new StateEntityKey { Entity = stateData.StateEntity, HashCode = stateData.GetHashCode()};
        }

        public StateData CopyStateData(StateData stateData)
        {
            var copyStateEntity = EntityManager.Instantiate(stateData.StateEntity);
            return new StateData(this, copyStateEntity, true);
        }

        public StateEntityKey CopyState(StateEntityKey stateKey)
        {
            var copyStateEntity = EntityManager.Instantiate(stateKey.Entity);
            var stateData = GetStateData(stateKey);
            return new StateEntityKey { Entity = copyStateEntity, HashCode = stateData.GetHashCode()};
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps) => inputDeps;

        public bool Equals(StateData x, StateData y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(StateData obj)
        {
            return obj.GetHashCode();
        }

        public IStateData GetStateData(IStateKey stateKey, bool readWrite)
        {
            return GetStateData((StateEntityKey)stateKey, readWrite);
        }
    }
}
