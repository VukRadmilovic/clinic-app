﻿using System;
using System.Collections.Generic;
using HealthCareCenter.Enums;

namespace HealthCareCenter.Model
{
    public class Patient : User
    {
        public bool IsBlocked { get; set; }
        public Blocker BlockedBy { get; set; }
        public List<int> ReferralIDs { get; set; }
        public List<int> PrescriptionIDs { get; set; }
        public int HealthRecordID { get; set; }
        
        //public List<Survey> Surveys { get; set; }
    }
}