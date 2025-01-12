using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace NodeSys
{
    public class temp : MonoBehaviour
    {
        public static void TestMe(int i)
        {
            Debug.Log(i.ToString());
        }
    }

    public class TestSerializedObject : ScriptableObject
    {
        public UnityEvent<int> testEvent = new UnityEvent<int>();
        void Temp()
        {
            testEvent.AddListener(Other);
        }

        void Other(int i)
        {

        }
    }
}
