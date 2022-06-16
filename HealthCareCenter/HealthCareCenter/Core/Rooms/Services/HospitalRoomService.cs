﻿using HealthCareCenter.Core.Appointments.Models;
using HealthCareCenter.Core.Appointments.Repository;
using HealthCareCenter.Core.Exceptions;
using HealthCareCenter.Core.Rooms.Models;
using HealthCareCenter.Core.Rooms.Repositories;
using System;
using System.Collections.Generic;

namespace HealthCareCenter.Core.Rooms.Services
{
    internal class HospitalRoomService
    {
        // change to parameterized constructor when refactoring starts
        private static readonly BaseAppointmentRepository _appointmentRepository = new AppointmentRepository();

        // promeniti da ne bude staticko
        private static readonly IHospitalRoomForRenovationService _hospitalRoomForRenovationService = new HospitalRoomForRenovationService(new HospitalRoomForRenovationRepository());

        public HospitalRoomService(IHospitalRoomForRenovationService hospitalRoomForRenovationService, BaseHospitalRoomForRenovationRepository hospitalRoomForRenovationRepository)
        {
            // Odkomentarisati kada se refaktorise
            //_hospitalRoomForRenovationService = hospitalRoomForRenovationService;
        }

        /// <summary>
        /// Return loaded hospital rooms from list.
        /// </summary>
        /// <returns>Loaded hospital rooms.</returns>
        public static List<HospitalRoom> GetRooms()
        {
            return HospitalRoomRepository.Rooms;
        }

        /// <summary>
        /// Finding room with specific id.
        /// </summary>
        /// <param name="id">id of wanted hospital room.</param>
        /// <returns>Hospital room with specific id, if room is found, or null if room is not found.</returns>
        /// <exception cref="HospitalRoomNotFound">Thrown when room with specific id is not found.</exception>
        public static HospitalRoom Get(int id)
        {
            try
            {
                foreach (HospitalRoom room in HospitalRoomRepository.Rooms)
                {
                    if (room.ID == id)
                    {
                        return room;
                    }
                }

                throw new HospitalRoomNotFound();
            }
            catch (HospitalRoomNotFound ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Add new hospital room in file hospitalRooms.json.
        /// </summary>
        /// <param name="newRoom"></param>
        public static void Add(HospitalRoom newRoom)
        {
            HospitalRoomRepository.Rooms.Add(newRoom);
            HospitalRoomRepository.Save();
        }

        public static void Insert(HospitalRoom room)
        {
            HospitalRoomRepository.Rooms.Add(room);
            HospitalRoomRepository.Rooms.Sort((x, y) => x.ID.CompareTo(y.ID));
            HospitalRoomRepository.Save();
        }

        /// <summary>
        /// Delete room from file HospitalRooms.josn with specific id.
        /// </summary>
        /// <param name="id">id of the hospital room we want to delete.</param>
        /// <returns>True if room is deleted or false if it's not.</returns>
        /// <exception cref="HospitalRoomNotFound">Thrown when room with specific id is not found.</exception>
        public static bool Delete(int id)
        {
            try
            {
                for (int i = 0; i < HospitalRoomRepository.Rooms.Count; i++)
                {
                    if (id == HospitalRoomRepository.Rooms[i].ID)
                    {
                        HospitalRoomRepository.Rooms.RemoveAt(i);
                        HospitalRoomRepository.Save();
                        return true;
                    }
                }
                throw new HospitalRoomNotFound();
            }
            catch (HospitalRoomNotFound ex)
            {
                Console.WriteLine(ex);
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Delete room from file HospitalRooms.josn with specific id.
        /// </summary>
        /// <param name="room">Room we want to delete.</param>
        /// <returns>true if room is deleted or false if it's not.</returns>
        /// <exception cref="HospitalRoomNotFound">Thrown when room is not found.</exception>
        public static bool Delete(HospitalRoom room)
        {
            try
            {
                for (int i = 0; i < HospitalRoomRepository.Rooms.Count; i++)
                {
                    if (room.ID == HospitalRoomRepository.Rooms[i].ID)
                    {
                        HospitalRoomRepository.Rooms.RemoveAt(i);
                        HospitalRoomRepository.Save();
                        return true;
                    }
                }
                throw new HospitalRoomNotFound();
            }
            catch (HospitalRoomNotFound ex)
            {
                Console.WriteLine(ex);
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Updating hospital room.
        /// </summary>
        /// <param name="room">Hospital room we want to update.</param>
        /// <returns>true if room is updated or false if room is not found.</returns>
        public static bool Update(HospitalRoom room)
        {
            try
            {
                for (int i = 0; i < HospitalRoomRepository.Rooms.Count; i++)
                {
                    if (room.ID == HospitalRoomRepository.Rooms[i].ID)
                    {
                        HospitalRoomRepository.Rooms[i] = room;
                        HospitalRoomRepository.Save();
                        return true;
                    }
                }
                throw new HospitalRoomNotFound();
            }
            catch (HospitalRoomNotFound ex)
            {
                Console.WriteLine(ex);
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void Update(int roomID, Appointment appointment)
        {
            foreach (HospitalRoom room in HospitalRoomRepository.Rooms)
            {
                if (room.ID == roomID)
                {
                    room.AppointmentIDs.Add(appointment.ID);
                    return;
                }
            }
        }

        public static int GetAvailableRoomID(DateTime scheduledDate, RoomType roomType)
        {
            int hospitalRoomID = -1;
            foreach (HospitalRoom hospitalRoom in HospitalRoomRepository.Rooms)
            {
                if (hospitalRoom.Type != roomType)
                {
                    continue;
                }

                hospitalRoomID = hospitalRoom.ID;
                foreach (Appointment appointment in _appointmentRepository.Appointments)
                {
                    if (hospitalRoom.AppointmentIDs.Contains(appointment.ID) && appointment.ScheduledDate.CompareTo(scheduledDate) == 0)
                    {
                        hospitalRoomID = -1;
                    }
                }
                if (hospitalRoomID != -1)
                {
                    break;
                }
            }

            return hospitalRoomID;
        }

        public static void AddAppointmentToRoom(int hospitalRoomID, int appointmentID)
        {
            foreach (HospitalRoom hospitalRoom in HospitalRoomRepository.Rooms)
            {
                if (hospitalRoom.ID == hospitalRoomID)
                {
                    hospitalRoom.AppointmentIDs.Add(appointmentID);
                    break;
                }
            }
            HospitalRoomRepository.Save();
        }

        public static bool IsCurrentlyRenovating(HospitalRoom room)
        {
            foreach (HospitalRoom hospitalRoom in _hospitalRoomForRenovationService.GetRooms())
            {
                if (room.ID == hospitalRoom.ID)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Check if room contains any appointment
        /// </summary>
        /// <returns></returns>
        public static bool ContainsAnyAppointment(HospitalRoom room)
        {
            return room.AppointmentIDs.Count != 0;
        }

        public static List<HospitalRoomForDisplay> GetRooms(bool checkup)
        {
            List<HospitalRoomForDisplay> rooms = new List<HospitalRoomForDisplay>();
            foreach (HospitalRoom room in HospitalRoomRepository.Rooms)
            {
                bool correctRoom = room.Type == RoomType.Checkup && checkup || room.Type == RoomType.Operation && !checkup;
                if (correctRoom)
                {
                    rooms.Add(new HospitalRoomForDisplay() { ID = room.ID, Name = room.Name });
                }
            }
            return rooms;
        }

        public static List<HospitalRoom> GetRoomsOfType(AppointmentType type)
        {
            List<HospitalRoom> rooms = new List<HospitalRoom>();
            foreach (HospitalRoom room in HospitalRoomRepository.Rooms)
            {
                bool correctRoom = type == AppointmentType.Checkup && room.Type == RoomType.Checkup || type == AppointmentType.Operation && room.Type == RoomType.Operation;
                if (correctRoom)
                {
                    rooms.Add(room);
                }
            }
            return rooms;
        }

        public static bool IsOccupied(int id, DateTime time)
        {
            foreach (Appointment appointment in _appointmentRepository.Appointments)
            {
                if (appointment.HospitalRoomID != id)
                {
                    continue;
                }
                if (appointment.ScheduledDate.CompareTo(time) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        public static void RemoveUnavailableRooms(List<HospitalRoom> availableRooms, Appointment appointment)
        {
            foreach (HospitalRoom room in availableRooms)
            {
                if (room.ID == appointment.HospitalRoomID)
                {
                    availableRooms.Remove(room);
                    return;
                }
            }
        }
    }
}