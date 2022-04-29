﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HealthCareCenter.Model
{
    public class EquipmentRearrangementRepository
    {
        public static List<EquipmentRearrangement> Rearrangements = LoadRearrangments();

        /// <summary>
        /// Loads all rearrangements from file equipmentRearrangement.json.
        /// </summary>
        /// <returns>List of all rearrangements.</returns>
        private static List<EquipmentRearrangement> LoadRearrangments()
        {
            try
            {
                List<EquipmentRearrangement> rearrangments = new List<EquipmentRearrangement>();
                var settings = new JsonSerializerSettings
                {
                    DateFormatString = Constants.DateFormat
                };

                string JSONTextEquipmentRearrangments = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"..\..\..\data\equipmentRearrangement.json");
                rearrangments = (List<EquipmentRearrangement>)JsonConvert.DeserializeObject<IEnumerable<EquipmentRearrangement>>(JSONTextEquipmentRearrangments, settings);
                return rearrangments;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Override content of file equipmentRearrangement.json with rearrangements list.
        /// </summary>
        /// <param name="rearrangements"></param>
        /// <returns>True if content override is ended successfully.</returns>
        public static bool SaveRearrangements(List<EquipmentRearrangement> rearrangements)
        {
            try
            {
                JsonSerializer serializer = new JsonSerializer();

                using (StreamWriter sw = new StreamWriter(@"..\..\..\data\equipmentRearrangement.json"))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, rearrangements);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}