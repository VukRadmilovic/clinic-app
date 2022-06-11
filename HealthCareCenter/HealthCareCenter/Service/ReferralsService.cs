﻿using HealthCareCenter.Model;
using HealthCareCenter.Secretary;
using System.Collections.Generic;

namespace HealthCareCenter.Service
{
    public class ReferralsService : IReferralsService
    {
        private BaseReferralRepository _referralsRepository;

        public ReferralsService(BaseReferralRepository repository)
        {
            _referralsRepository = repository;
        }

        public List<PatientReferralForDisplay> Get(Patient patient)
        {
            List<PatientReferralForDisplay> referrals = new List<PatientReferralForDisplay>();
            foreach (Referral referral in _referralsRepository.Referrals)
            {
                if (referral.PatientID != patient.ID)
                {
                    continue;
                }

                Add(referral, referrals);
            }
            return referrals;
        }

        private void Add(Referral referral, List<PatientReferralForDisplay> referrals)
        {
            PatientReferralForDisplay patientReferral = new PatientReferralForDisplay(referral.ID);

            LinkDoctor(referral, patientReferral);
            referrals.Add(patientReferral);
        }

        private void LinkDoctor(Referral referral, PatientReferralForDisplay patientReferral)
        {
            foreach (Doctor doctor in UserRepository.Doctors)
            {
                if (doctor.ID != referral.DoctorID)
                {
                    continue;
                }
                patientReferral.DoctorUsername = doctor.Username;
                patientReferral.DoctorFirstName = doctor.FirstName;
                patientReferral.DoctorLastName = doctor.LastName;
                return;
            }
        }

        public Referral Get(int referralID)
        {
            foreach (Referral referral in _referralsRepository.Referrals)
            {
                if (referral.ID == referralID)
                {
                    return referral;
                }
            }
            return null;
        }

        public void Schedule(Referral referral, Appointment appointment)
        {
            AppointmentRepository.Appointments.Add(appointment);
            AppointmentRepository.Save();

            HospitalRoomService.Update(appointment.HospitalRoomID, appointment);
            HospitalRoomRepository.Save();

            _referralsRepository.Referrals.Remove(referral);
            _referralsRepository.Save();
        }

        public void Fill(int doctorID, int patientID, Referral referral)
        {
            referral.ID = _referralsRepository.LargestID;
            referral.DoctorID = doctorID;
            referral.PatientID = patientID;
        }
    }
}
