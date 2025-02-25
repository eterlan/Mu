﻿namespace Unity.AI.Planner
{
    /// <summary>
    /// An interface that marks an implementation of a heuristic for estimating the value of a state in a specific domain
    /// </summary>
    /// <typeparam name="TStateData"></typeparam>
    public interface IHeuristic<TStateData>
        where TStateData : struct
    {
        /// <summary>
        /// Evaluate a state to provide an estimate
        /// </summary>
        /// <param name="stateData">State to evaluate</param>
        /// <returns>Value estimate of the state</returns>
        float Evaluate(TStateData stateData);
    }
}
