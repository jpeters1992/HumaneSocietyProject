﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    public static class Query
    {        
        static HumaneSocietyDataContext db;

        static Query()
        {
            db = new HumaneSocietyDataContext();
        }

        internal static List<USState> GetStates()
        {
            List<USState> allStates = db.USStates.ToList();       

            return allStates;
        }
            
        internal static Client GetClient(string userName, string password)
        {
            Client client = db.Clients.Where(c => c.UserName == userName && c.Password == password).Single();

            return client;
        }

        internal static List<Client> GetClients()
        {
            List<Client> allClients = db.Clients.ToList();

            return allClients;
        }

        internal static void AddNewClient(string firstName, string lastName, string username, string password, string email, string streetAddress, int zipCode, int stateId)
        {
            Client newClient = new Client();

            newClient.FirstName = firstName;
            newClient.LastName = lastName;
            newClient.UserName = username;
            newClient.Password = password;
            newClient.Email = email;

            Address addressFromDb = db.Addresses.Where(a => a.AddressLine1 == streetAddress && a.Zipcode == zipCode && a.USStateId == stateId).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if (addressFromDb == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = streetAddress;
                newAddress.City = null;
                newAddress.USStateId = stateId;
                newAddress.Zipcode = zipCode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                addressFromDb = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            newClient.AddressId = addressFromDb.AddressId;

            db.Clients.InsertOnSubmit(newClient);

            db.SubmitChanges();
        }

        internal static void UpdateClient(Client clientWithUpdates)
        {
            // find corresponding Client from Db
            Client clientFromDb = null;

            try
            {
                clientFromDb = db.Clients.Where(c => c.ClientId == clientWithUpdates.ClientId).Single();
            }
            catch(InvalidOperationException e)
            {
                Console.WriteLine("No clients have a ClientId that matches the Client passed in.");
                Console.WriteLine("No update have been made.");
                return;
            }
            
            // update clientFromDb information with the values on clientWithUpdates (aside from address)
            clientFromDb.FirstName = clientWithUpdates.FirstName;
            clientFromDb.LastName = clientWithUpdates.LastName;
            clientFromDb.UserName = clientWithUpdates.UserName;
            clientFromDb.Password = clientWithUpdates.Password;
            clientFromDb.Email = clientWithUpdates.Email;

            // get address object from clientWithUpdates
            Address clientAddress = clientWithUpdates.Address;

            // look for existing Address in Db (null will be returned if the address isn't already in the Db
            Address updatedAddress = db.Addresses.Where(a => a.AddressLine1 == clientAddress.AddressLine1 && a.USStateId == clientAddress.USStateId && a.Zipcode == clientAddress.Zipcode).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if(updatedAddress == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = clientAddress.AddressLine1;
                newAddress.City = null;
                newAddress.USStateId = clientAddress.USStateId;
                newAddress.Zipcode = clientAddress.Zipcode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                updatedAddress = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            clientFromDb.AddressId = updatedAddress.AddressId;
            
            // submit changes
            db.SubmitChanges();
        }
        
        internal static void AddUsernameAndPassword(Employee employee)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).FirstOrDefault();

            employeeFromDb.UserName = employee.UserName;
            employeeFromDb.Password = employee.Password;

            db.SubmitChanges();
        }

        internal static Employee RetrieveEmployeeUser(string email, int employeeNumber)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.Email == email && e.EmployeeNumber == employeeNumber).FirstOrDefault();

            if (employeeFromDb == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return employeeFromDb;
            }
        }

        internal static Employee EmployeeLogin(string userName, string password)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.UserName == userName && e.Password == password).FirstOrDefault();

            return employeeFromDb;
        }

        internal static bool CheckEmployeeUserNameExist(string userName)
        {
            Employee employeeWithUserName = db.Employees.Where(e => e.UserName == userName).FirstOrDefault();
            return employeeWithUserName == null;
        }

       //Allow any of the CRUD operations to occur here
        internal static void RunEmployeeQueries(Employee employee, string crudOperation)
        {
            switch (crudOperation)
            {
                case "create":
                    db.Employees.InsertOnSubmit(employee);
                    db.SubmitChanges();
                    break;
                case "read":
                    UserInterface.DisplayEmployeeInfo(employee);
                    break;
                case "update":
                    Employee updatedEmployee = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).Single();
                    updatedEmployee.FirstName = employee.FirstName;
                    updatedEmployee.LastName = employee.LastName;
                    updatedEmployee.EmployeeNumber = employee.EmployeeNumber;
                    updatedEmployee.Email = employee.Email;
                    db.SubmitChanges();
                    break;
                case "delete":
                    db.Employees.DeleteOnSubmit(employee);
                    db.SubmitChanges();
                    break;
                default:
                    break;
            }
        }

        //Animal CRUD Operations
        internal static void AddAnimal(Animal animal)
        {
            db.Animals.InsertOnSubmit(animal);
            db.SubmitChanges();
        }

        internal static Animal GetAnimalByID(int id)
        {
            return db.Animals.Where(e => e.AnimalId == id).FirstOrDefault();
        }

        internal static void UpdateAnimal(int animalId, Dictionary<int, string> updates)
        {
            Animal animal = GetAnimalByID(animalId);
            foreach (KeyValuePair<int, string> submission in updates)
            {
                switch (submission.Key)
                {
                    case 1:
                        animal.CategoryId = GetCategoryId(submission.Value);
                        return;
                    case 2:
                        animal.Name = submission.Value;
                        return;
                    case 3:
                        animal.Age = Convert.ToInt32(submission.Value);
                        return;
                    case 4:
                        animal.Demeanor = submission.Value;
                        return;
                    case 5:
                        animal.KidFriendly = Convert.ToBoolean(submission.Value);
                        return;
                    case 6:
                        animal.PetFriendly = Convert.ToBoolean(submission.Value);
                        return;
                    case 7:
                        animal.Weight = Convert.ToInt32(submission.Value);
                        return;
                    case 8:
                        animal.AnimalId = Convert.ToInt32(submission.Value);
                        return;
                    default:
                        return;
                }
            }
            db.Animals.InsertOnSubmit(animal);
            db.SubmitChanges();
        }

        internal static void RemoveAnimal(Animal animal)
        {
            db.Animals.DeleteOnSubmit(animal);
            db.SubmitChanges();
        }
        
        //Animal Multi-Trait Search
        internal static IQueryable<Animal> SearchForAnimalsByMultipleTraits(Dictionary<int, string> updates) // parameter(s)?
        {
            IQueryable<Animal> animals = db.Animals;

            foreach (KeyValuePair<int, string> item in updates)
            {
                switch (item.Key)
                {
                    case 1:
                        animals = animals.Where(a => a.CategoryId == GetCategoryId(item.Value));
                        break;
                    case 2:
                        animals = animals.Where(a => a.Name == item.Value);
                        break;
                    case 3:
                        animals = animals.Where(a => a.Age == Convert.ToInt32(item.Value));
                        break;
                    case 4:
                        animals = animals.Where(a => a.Demeanor == item.Value);
                        break;
                    case 5:
                        animals = animals.Where(a => a.KidFriendly == Convert.ToBoolean(item.Value));
                        break;
                    case 6:
                        animals = animals.Where(a => a.PetFriendly == Convert.ToBoolean(item.Value));
                        break;
                    case 7:
                        animals = animals.Where(a => a.Weight == Convert.ToInt32(item.Value));
                        break;
                    case 8:
                        animals = animals.Where(a => a.AnimalId == Convert.ToInt32(item.Value));
                        break;
                }
            }
            return animals;
        }
         
        //Misc Animal Things
        internal static int GetCategoryId(string categoryName)
        {
            return db.Categories.Where(c => c.Name == categoryName).Select(c => c.CategoryId).FirstOrDefault();
        }
        
        internal static Room GetRoom(int animalId)
        {
            return db.Rooms.Where(r => r.AnimalId == animalId).FirstOrDefault();
        } 
        
        internal static int GetDietPlanId(string dietPlanName)
        {
            return db.DietPlans.Where(p => p.Name == dietPlanName).Select(p => p.DietPlanId).FirstOrDefault();
        }

        //Adoption CRUD Operation
        internal static void Adopt(Animal animal, Client client)
        {
            Adoption adoption = new Adoption();
            adoption.ApprovalStatus = "pending";
            adoption.AnimalId = animal.AnimalId;
            adoption.ClientId = client.ClientId;
            adoption.AdoptionFee = 25;
            adoption.PaymentCollected = false;

            db.Adoptions.InsertOnSubmit(adoption);
            db.SubmitChanges();
        }

        internal static IQueryable<Adoption> GetPendingAdoptions()
        {
            return db.Adoptions.Where(a => a.ApprovalStatus == "pending");
        }

        internal static void UpdateAdoption(bool isAdopted, Adoption adoption)
        {
            //use ternary instead?
            if (isAdopted)
            {
                adoption.ApprovalStatus = "Approved";

                db.SubmitChanges();
            }
            else
            {
                adoption.ApprovalStatus = "Denied";
                RemoveAdoption(adoption.AnimalId, adoption.ClientId);
            }
        }

        internal static void RemoveAdoption(int animalId, int clientId)
        {
            Adoption adoption = db.Adoptions.Where(a => a.AnimalId == animalId && a.ClientId == clientId).SingleOrDefault();

            db.Adoptions.DeleteOnSubmit(adoption);
            db.SubmitChanges();
        }

        //Shots Stuff
        internal static IQueryable<AnimalShot> GetShots(Animal animal)
        {
            return db.AnimalShots.Where(aS => aS.AnimalId == animal.AnimalId);
        }

        internal static void UpdateShot(string shotName, Animal animal)
        {
            AnimalShot newShots = new AnimalShot();

            newShots.AnimalId = animal.AnimalId;
            newShots.ShotId = db.Shots.Where(s => s.Name == shotName).Select(s => s.ShotId).SingleOrDefault();
            newShots.DateReceived = DateTime.Now;

            db.AnimalShots.InsertOnSubmit(newShots);
            db.SubmitChanges();
        }
    }
}