/*
Copyright (c) 2025 Valem Studio

This asset is the intellectual property of Valem Studio and is distributed under the Unity Asset Store End User License Agreement (EULA).

Unauthorized reproduction, modification, or redistribution of any part of this asset outside the terms of the Unity Asset Store EULA is strictly prohibited.

For support or inquiries, please contact Valem Studio via social media or through the publisher profile on the Unity Asset Store.
*/

using System.Linq;
using UnityEngine;
namespace AVRO
{
    [CreateAssetMenu(fileName = "Ticket", menuName = "AutoVROptimizer/CreateTicket", order = 1)]
    public class AVRO_Ticket : ScriptableObject
    {
        public AVRO_Settings.Ticket data;
        [HideInInspector] public bool IsBigTicket;

        public void AddObjectGUID(string _id, int _value = -1)
        {
            AVRO_Settings.Ticket.ConcernedObjectData _new = new AVRO_Settings.Ticket.ConcernedObjectData();
            _new.guid = _id;
            _new.toggle = false;
            _new.value = _value;
            data.concernedObjects.Add(_new);
            data.concernedObjects = data.concernedObjects.OrderByDescending(obj => obj.value).ToList();
        }

        [ContextMenu("SetDataNameFromObject")]
        public void SetDataNameFromObject()
        {
            data.name = name.Split('-')[1];
            data.name = data.name.Remove(0, 1);
        }

        [ContextMenu("RemoveTagFromName")]
        public void RemoveTagFromName()
        {
            data.name = data.name.Split(']')[1];
            data.name = data.name.Remove(0, 1);
        }

        [ContextMenu("AddTagToName")]
        public void AddTagToName()
        {
            data.name = "[" + name.Split(' ')[0] + "] " + data.name;
        }
    }
}