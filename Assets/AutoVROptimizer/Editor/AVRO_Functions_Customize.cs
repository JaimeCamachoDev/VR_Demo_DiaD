/*
Copyright (c) 2025 Valem Studio

This asset is the intellectual property of Valem Studio and is distributed under the Unity Asset Store End User License Agreement (EULA).

Unauthorized reproduction, modification, or redistribution of any part of this asset outside the terms of the Unity Asset Store EULA is strictly prohibited.

For support or inquiries, please contact Valem Studio via social media or through the publisher profile on the Unity Asset Store.
*/

namespace AVRO
{
    public class AVRO_Functions_Customize
    {
        // Add your custom functions here
        public static AVRO_Settings.TicketStates GetYourFunctionName(AVRO_Ticket _ticket)
        {
            // Check for your conditions
            bool _condition = true; // Replace with your actual condition
            return _condition ? AVRO_Settings.TicketStates.Done : AVRO_Settings.TicketStates.Todo;
        }
        public static void SetYourFunctionName(AVRO_Ticket _ticket)
        {
            // Apply your changes
        }
    }
}
