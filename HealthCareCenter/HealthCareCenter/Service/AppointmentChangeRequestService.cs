﻿using HealthCareCenter.Enums;
using HealthCareCenter.Secretary;
using HealthCareCenter.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCareCenter.Model
{
    public static class AppointmentChangeRequestService
    {
        public static void DeleteAppointment(AppointmentChangeRequest request)
        {
            if (AppointmentRepository.Appointments == null)
            {
                return;
            }

            foreach (Appointment appointment in AppointmentRepository.Appointments)
            {
                if (appointment.ID == request.AppointmentID)
                {
                    HospitalRoomService.Get(appointment.HospitalRoomID).AppointmentIDs.Remove(appointment.ID);
                    AppointmentRepository.Appointments.Remove(appointment);
                    break;
                }
            }
        }

        public static void EditAppointment(AppointmentChangeRequest request)
        {
            if (AppointmentRepository.Appointments == null)
            {
                return;
            }

            foreach (Appointment appointment in AppointmentRepository.Appointments)
            {
                if (appointment.ID == request.AppointmentID)
                {
                    appointment.ScheduledDate = request.NewDate;
                    appointment.Type = request.NewAppointmentType;
                    appointment.DoctorID = request.NewDoctorID;
                    break;
                }
            }
        }

        public static void Refresh(List<DeleteRequest> deleteRequests, List<EditRequest> editRequests, Patient patient)
        {
            foreach (AppointmentChangeRequest request in AppointmentChangeRequestRepository.Requests)
            {
                if (request.State != RequestState.Waiting || request.PatientID != patient.ID)
                {
                    continue;
                }
                if (request.RequestType == RequestType.Delete)
                {
                    AddDeleteRequest(request, deleteRequests);
                }
                else
                {
                    AddEditRequest(request, editRequests);
                }
            }
        }

        private static void AddEditRequest(AppointmentChangeRequest request, List<EditRequest> editRequests)
        {
            EditRequest editRequest = new EditRequest(request.ID, request.DateSent);

            foreach (Appointment appointment in AppointmentRepository.Appointments)
            {
                if (appointment.ID != request.AppointmentID)
                {
                    continue;
                }

                LinkDoctor(request, editRequest, appointment);

                editRequest.OriginalAppointmentTime = appointment.ScheduledDate;
                editRequest.OriginalType = appointment.Type;
                break;
            }
            editRequest.NewAppointmentTime = request.NewDate;
            editRequest.NewType = request.NewAppointmentType;
            editRequests.Add(editRequest);
        }

        private static void LinkDoctor(AppointmentChangeRequest request, EditRequest editRequest, Appointment appointment)
        {
            bool foundOld = false;
            bool foundNew = false;
            foreach (Doctor doctor in UserRepository.Doctors)
            {
                if (doctor.ID == appointment.DoctorID)
                {
                    editRequest.OriginalDoctorUsername = doctor.Username;
                    foundOld = true;
                }
                if (doctor.ID == request.NewDoctorID)
                {
                    editRequest.NewDoctorUsername = doctor.Username;
                    foundNew = true;
                }
                if (foundNew && foundOld)
                {
                    return;
                }
            }
        }

        private static void AddDeleteRequest(AppointmentChangeRequest request, List<DeleteRequest> deleteRequests)
        {
            DeleteRequest deleteRequest = new DeleteRequest(request.ID, request.DateSent);

            foreach (Appointment appointment in AppointmentRepository.Appointments)
            {
                if (appointment.ID != request.AppointmentID)
                {
                    continue;
                }

                LinkDoctor(deleteRequest, appointment);
                deleteRequest.AppointmentTime = appointment.ScheduledDate;
                break;
            }
            deleteRequests.Add(deleteRequest);
        }

        private static void LinkDoctor(DeleteRequest deleteRequest, Appointment appointment)
        {
            foreach (Doctor doctor in UserRepository.Doctors)
            {
                if (doctor.ID == appointment.DoctorID)
                {
                    deleteRequest.DoctorUsername = doctor.Username;
                    return;
                }
            }
        }

        public static void RejectEditRequest(int requestID)
        {
            Find(requestID).State = RequestState.Denied;
            AppointmentChangeRequestRepository.Save();
        }

        public static void AcceptEditRequest(int requestID)
        {
            AppointmentChangeRequest request = Find(requestID);
            request.State = RequestState.Approved;
            EditAppointment(request);

            AppointmentRepository.Save();
            AppointmentChangeRequestRepository.Save();
        }

        public static void RejectDeleteRequest(int requestID)
        {
            Find(requestID).State = RequestState.Denied;
            AppointmentChangeRequestRepository.Save();
        }

        public static void AcceptDeleteRequest(int requestID)
        {
            AppointmentChangeRequest request = Find(requestID);
            request.State = RequestState.Approved;
            DeleteAppointment(request);

            AppointmentRepository.Save();
            AppointmentChangeRequestRepository.Save();
        }

        private static AppointmentChangeRequest Find(int requestID)
        {
            foreach (AppointmentChangeRequest request in AppointmentChangeRequestRepository.Requests)
            {
                if (request.ID == requestID)
                {
                    return request;
                }
            }
            return null;
        }
    }
}
