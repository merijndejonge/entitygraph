using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenSoftware.DataValidationFramework.Net;
using OpenSoftware.DataValidationFrameworkCore;
using OpenSoftware.EntityGraph.Net.Tests.Annotations;
using OpenSoftware.EntityGraphCore;

namespace OpenSoftware.EntityGraph.Net.Tests.Tests
{
    public class Wheel
    {
    }

    public enum EngineType
    {
        Diesel,
        Benzin,
        Gaz
    }

    public class Engine : INotifyPropertyChanged
    {
        private EngineType _engineType;

        public EngineType EngineType
        {
            get => _engineType;
            set
            {
                if (_engineType == value) return;
                _engineType = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Door
    {
    }

    public abstract class Car : INotifyPropertyChanged
    {
        private string _id;

        public string Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<Wheel> Wheels { get; set; }
        public ObservableCollection<Door> Doors { get; set; }
        public Engine Engine { get; set; }
        public Owner Owner { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Owner
    {
    }

    public class Trailer
    {
    }

    public class Truck : Car
    {
        public Truck()
        {
            Wheels = new ObservableCollection<Wheel>
            {
                new Wheel(),
                new Wheel(),
                new Wheel(),
                new Wheel(),
                new Wheel(),
                new Wheel()
            };
            Doors = new ObservableCollection<Door>
            {
                new Door(),
                new Door()
            };
            Engine = new Engine {EngineType = EngineType.Diesel};
        }

        private Trailer _trailer;

        public Trailer Trailer
        {
            get => _trailer;
            set
            {
                if (_trailer == value) return;
                _trailer = value;
                OnPropertyChanged();
            }
        }
    }

    public class PersonCar : Car
    {
        public PersonCar()
        {
            Wheels = new ObservableCollection<Wheel>
            {
                new Wheel(),
                new Wheel(),
                new Wheel(),
                new Wheel()
            };
            Doors = new ObservableCollection<Door>
            {
                new Door(),
                new Door(),
                new Door(),
                new Door()
            };
            Engine = new Engine {EngineType = EngineType.Benzin};
        }
    }

    public class CarPark
    {
        public ObservableCollection<Car> Cars { get; set; }

        public static readonly EntityGraphShape Shape =
            new EntityGraphShape()
                .Edge<CarPark, Car>(carPark => carPark.Cars)
                .Edge<Car, Wheel>(car => car.Wheels)
                .Edge<Car, Door>(car => car.Doors)
                .Edge<Car, Engine>(car => car.Engine)
                .Edge<Truck, Trailer>(truck => truck.Trailer);
    }

    public class UniqIds : ValidationRule
    {
        public UniqIds()
            : base(
                InputOutput<Car, string>(car1 => car1.Id),
                InputOutput<Car, string>(car2 => car2.Id)
            )
        {
        }

        public ValidationResult Validate(string carId1, string carId2)
        {
            return carId1 == carId2 ? new ValidationResult("Car ids should be uniqe") : ValidationResult.Success;
        }
    }

    public class TruckDoorsValidator : ValidationRule
    {
        public TruckDoorsValidator() :
            base(InputOutput<Truck, IEnumerable<Door>>(truck => truck.Doors))
        {
        }

        public ValidationResult Validate(IEnumerable<Door> doors)
        {
            return doors.Count() > 2 ? new ValidationResult("Truck has max 2 doors.") : ValidationResult.Success;
        }
    }

    public class TruckEngineValidator : ValidationRule
    {
        public TruckEngineValidator() :
            base(
                InputOutput<Truck, Engine>(truck => truck.Engine),
                InputOnly<Truck, EngineType>(truck => truck.Engine.EngineType)
            )
        {
        }

        public ValidationResult Validate(Engine engine, EngineType engineType)
        {
            return engineType != EngineType.Diesel
                ? new ValidationResult("Truck should have a diesel engine.")
                : ValidationResult.Success;
        }
    }

    public class TruckWheelsValidator : ValidationRule
    {
        public TruckWheelsValidator() :
            base(InputOutput<Truck, IEnumerable<Wheel>>(truck => truck.Wheels)
            )
        {
        }

        public ValidationResult Validate(IEnumerable<Wheel> wheels)
        {
            if (wheels.Count() > 4) return ValidationResult.Success;
            return new ValidationResult("Truck should have at least 4 wheels.");
        }
    }

    public class TruckTrailerValidator : ValidationRule
    {
        public TruckTrailerValidator() :
            base(
                InputOutput<Truck, Trailer>(truck1 => truck1.Trailer),
                InputOutput<Truck, Trailer>(truck2 => truck2.Trailer)
            )
        {
        }

        public ValidationResult Validate(Trailer trailer1, Trailer trailer2)
        {
            if (trailer1 != null && trailer1 == trailer2)
            {
                return new ValidationResult("A trailer can be attached to a single truck only");
            }

            return ValidationResult.Success;
        }
    }

    [TestClass]
    public class CarsExampleTests
    {
        [TestMethod]
        public void UniqIdsTest()
        {
            var truck = new Truck {Id = "1"};
            var personCar = new PersonCar {Id = "2"};
            var carPark = new CarPark
            {
                Cars = new ObservableCollection<Car> {truck, personCar}
            };
            var gr = carPark.EntityGraph(CarPark.Shape);
            gr.Validator = new ValidationEngine
            {
                RulesProvider = new SimpleValidationRulesProvider<ValidationResult> {new UniqIds()}
            };

            Assert.IsFalse(gr.Validator.Entity(truck).HasErrors);
            Assert.IsFalse(gr.Validator.Entity(personCar).HasErrors);
            truck.Id = "2";
            Assert.IsTrue(gr.Validator.Entity(truck).HasErrors);
            Assert.IsTrue(gr.Validator.Entity(personCar).HasErrors);
            personCar.Id = "1";
            Assert.IsFalse(gr.Validator.Entity(truck).HasErrors);
            Assert.IsFalse(gr.Validator.Entity(personCar).HasErrors);

        }

        [TestMethod]
        public void TruckEquipmentTest()
        {
            var truck = new Truck {Id = "1"};
            var personCar = new PersonCar {Id = "2"};
            var carPark = new CarPark
            {
                Cars = new ObservableCollection<Car> {truck, personCar}
            };
            var gr = carPark.EntityGraph(CarPark.Shape);
            gr.Validator = new ValidationEngine
            {
                RulesProvider = new SimpleValidationRulesProvider<ValidationResult> {new TruckDoorsValidator()}
            };

            Assert.IsFalse(gr.Validator.Entity(truck).HasErrors);
            truck.Doors.Add(new Door());
            Assert.IsTrue(gr.Validator.Entity(truck).HasErrors);
        }

        [TestMethod]
        public void TruckEngineTest()
        {
            var truck = new Truck {Id = "1"};
            var personCar = new PersonCar {Id = "2"};
            var carPark = new CarPark
            {
                Cars = new ObservableCollection<Car> {truck, personCar}
            };
            var gr = carPark.EntityGraph(CarPark.Shape);
            gr.Validator = new ValidationEngine
            {
                RulesProvider = new SimpleValidationRulesProvider<ValidationResult> {new TruckEngineValidator()}
            };

            Assert.IsFalse(gr.Validator.Entity(truck).HasErrors);
            Assert.IsFalse(gr.Validator.Entity(truck.Engine).HasErrors);

            truck.Engine.EngineType = EngineType.Benzin;

            Assert.IsTrue(gr.Validator.Entity(truck).HasErrors);
            Assert.IsFalse(gr.Validator.Entity(truck.Engine).HasErrors);

            truck.Engine.EngineType = EngineType.Diesel;
            Assert.IsFalse(gr.Validator.Entity(truck).HasErrors);
            Assert.IsFalse(gr.Validator.Entity(truck.Engine).HasErrors);
        }

        [TestMethod]
        public void TruckTrailerTest()
        {
            var truck1 = new Truck {Id = "1"};
            var truck2 = new Truck {Id = "2"};
            var trailer = new Trailer();
            var carPark = new CarPark
            {
                Cars = new ObservableCollection<Car> {truck1, truck2}
            };
            var gr = carPark.EntityGraph(CarPark.Shape);
            gr.Validator = new ValidationEngine
            {
                RulesProvider = new SimpleValidationRulesProvider<ValidationResult> {new TruckTrailerValidator()}
            };

            truck1.Trailer = trailer;
            Assert.IsFalse(gr.Validator.Entity(truck1).HasErrors);
            Assert.IsFalse(gr.Validator.Entity(truck2).HasErrors);

            truck2.Trailer = trailer;
            Assert.IsTrue(gr.Validator.Entity(truck1).HasErrors);
            Assert.IsTrue(gr.Validator.Entity(truck2).HasErrors);
        }
    }
}