using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElectricPie.AStar
{
    [RequireComponent(typeof(AStarPathfinding))]
    public class PathRequestManager : MonoBehaviour
    {
        private AStarPathfinding m_pathfinder = null;

        private static PathRequestManager m_instance;
        
        private Queue<PathRequest> m_pathRequestQueue = new Queue<PathRequest>();
        private PathRequest m_currentPathRequest;
        private bool m_isProcessingPath;
        
        private struct PathRequest
        {
            public Vector3 PathStart { get; private set; }
            public Vector3 PathEnd { get; private set; }
            public Action<Vector3[], bool> Callback;

            public PathRequest(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
            {
                PathStart = pathStart;
                PathEnd = pathEnd;
                Callback = callback;
            }
        }

        private void Awake()
        {
            m_instance ??= this;
            m_pathfinder = GetComponent<AStarPathfinding>();
        }

        public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
        {
            PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
            m_instance.m_pathRequestQueue.Enqueue(newRequest);
            m_instance.TryProcessNext();
        }
        
        public void FinishedProcessingPath(Vector3[] path, bool pathFound)
        {
            m_currentPathRequest.Callback(path, pathFound);
            m_isProcessingPath = false;
            TryProcessNext();
        }

        private void TryProcessNext()
        {
            if (m_isProcessingPath == false && m_pathRequestQueue.Count > 0)
            {
                m_currentPathRequest = m_pathRequestQueue.Dequeue();
                m_isProcessingPath = true;
                m_pathfinder.StartFindPath(m_currentPathRequest.PathStart, m_currentPathRequest.PathEnd);
            }
        }
    }
}