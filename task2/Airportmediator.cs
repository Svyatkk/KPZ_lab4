using System;
using System.Collections.Generic;

namespace Lab4.Task2_Mediator
{
    public interface ICommandCentre
    {
        void RequestLanding(Aircraft aircraft);
        void RequestTakeoff(Aircraft aircraft);
        void NotifyRunwayFree(Runway runway);
        void RegisterRunway(Runway runway);
        void RegisterAircraft(Aircraft aircraft);
    }

    public class CommandCentre : ICommandCentre
    {
        private readonly List<Runway> _runways = new();
        private readonly Queue<(Aircraft aircraft, string action)> _waitingQueue = new();

        public void RegisterRunway(Runway runway)
        {
            _runways.Add(runway);
            Console.WriteLine($"[CommandCentre] Смугу '{runway.Name}' зареєстровано.");
        }

        public void RegisterAircraft(Aircraft aircraft)
        {
            Console.WriteLine($"[CommandCentre] Літак '{aircraft.FlightNumber}' зареєстровано.");
        }

        public void RequestLanding(Aircraft aircraft)
        {
            Console.WriteLine($"\n[CommandCentre] Отримано запит на посадку від '{aircraft.FlightNumber}'.");

            Runway? freeRunway = FindFreeRunway();
            if (freeRunway != null)
            {
                freeRunway.Occupy(aircraft.FlightNumber);
                aircraft.ConfirmLanding(freeRunway.Name);
            }
            else
            {
                Console.WriteLine($"[CommandCentre] Немає вільних смуг. Літак '{aircraft.FlightNumber}' очікує посадки.");
                _waitingQueue.Enqueue((aircraft, "landing"));
            }
        }

        public void RequestTakeoff(Aircraft aircraft)
        {
            Console.WriteLine($"\n[CommandCentre] Отримано запит на зліт від '{aircraft.FlightNumber}'.");

            Runway? freeRunway = FindFreeRunway();
            if (freeRunway != null)
            {
                freeRunway.Occupy(aircraft.FlightNumber);
                aircraft.ConfirmTakeoff(freeRunway.Name);
            }
            else
            {
                Console.WriteLine($"[CommandCentre] Немає вільних смуг. Літак '{aircraft.FlightNumber}' очікує зльоту.");
                _waitingQueue.Enqueue((aircraft, "takeoff"));
            }
        }

        public void NotifyRunwayFree(Runway runway)
        {
            Console.WriteLine($"[CommandCentre] Смуга '{runway.Name}' тепер вільна.");

            if (_waitingQueue.Count > 0)
            {
                var (nextAircraft, action) = _waitingQueue.Dequeue();
                runway.Occupy(nextAircraft.FlightNumber);

                if (action == "landing")
                    nextAircraft.ConfirmLanding(runway.Name);
                else
                    nextAircraft.ConfirmTakeoff(runway.Name);
            }
        }

        private Runway? FindFreeRunway()
        {
            foreach (var runway in _runways)
                if (runway.IsFree) return runway;
            return null;
        }
    }

    public class Aircraft
    {
        public string FlightNumber { get; }
        private readonly ICommandCentre _commandCentre;

        public Aircraft(string flightNumber, ICommandCentre commandCentre)
        {
            FlightNumber = flightNumber;
            _commandCentre = commandCentre;
            _commandCentre.RegisterAircraft(this);
        }

        public void RequestLanding()
        {
            Console.WriteLine($"[Aircraft {FlightNumber}] Запитую дозвіл на посадку...");
            _commandCentre.RequestLanding(this);
        }

        public void RequestTakeoff()
        {
            Console.WriteLine($"[Aircraft {FlightNumber}] Запитую дозвіл на зліт...");
            _commandCentre.RequestTakeoff(this);
        }

        public void ConfirmLanding(string runwayName)
        {
            Console.WriteLine($"[Aircraft {FlightNumber}] ✈️  Дозвіл на посадку отримано. Сідаю на смугу '{runwayName}'.");
        }

        public void ConfirmTakeoff(string runwayName)
        {
            Console.WriteLine($"[Aircraft {FlightNumber}] Дозвіл на зліт отримано. Злітаю зі смуги '{runwayName}'.");
        }
    }

    public class Runway
    {
        public string Name { get; }
        public bool IsFree { get; private set; } = true;
        private readonly ICommandCentre _commandCentre;

        public Runway(string name, ICommandCentre commandCentre)
        {
            Name = name;
            _commandCentre = commandCentre;
            _commandCentre.RegisterRunway(this);
        }

        public void Occupy(string flightNumber)
        {
            IsFree = false;
            Console.WriteLine($"[Runway {Name}] Зайнята літаком '{flightNumber}'.");
        }

        public void Release()
        {
            IsFree = true;
            Console.WriteLine($"[Runway {Name}] Звільнена. Повідомляю командний центр.");
            _commandCentre.NotifyRunwayFree(this);
        }
    }
}