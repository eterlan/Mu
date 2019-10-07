using Unity.AI.Planner;
using Unity.Collections;

namespace AI.Planner.Domains
{
    public struct Die : ITerminationEvaluator<StateData>
    {
        public bool IsTerminal(StateData state)
        {
            var NeedObjects = new NativeList<(DomainObject, int)>(2, Allocator.Temp);
            state.GetDomainObjects(NeedObjects, typeof(Need));
            var NeedBuffer = state.NeedBuffer;
            for (int i0 = 0; i0 < NeedObjects.Length; i0++)
            {
                var (NeedObject, NeedIndex) = NeedObjects[i0];

                
                    if (NeedBuffer[NeedObject.NeedIndex].Urgency >= 100)
                        return true;
            }
            NeedObjects.Dispose();

            return false;
        }
    }
}
