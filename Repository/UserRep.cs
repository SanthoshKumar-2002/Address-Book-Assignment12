using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Serialization;
using WebApi.Entities.DTO;
using WebApi.Entities.Models;
using WebApi2.Contracts;
using WebApi2.Entities;
using WebApi2.Entities.Models;

namespace WebApi2.Repository
{
    public class UserRep : IUserRep
    {
        private readonly ApiDbContext dbcontext;
        public UserRep(ApiDbContext context)
        {
            dbcontext = context;
        }
        
        /// <summary>
        /// to add a new address book
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Guid</returns>
        // to add the new user
        public Guid Create(User user)
        {
            dbcontext.Users.Add(user);
          
            return user.Id;
        }
        /// <summary>
        /// to delete by using the user id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Delete(Guid id)
        {
            User user = dbcontext.Users.FirstOrDefault(o =>o.Id == id);
            
            if (user == null) return false;
            if (user.IsActive==false) return false;
            user.IsActive=false;
            return true;
        }
        /// <summary>
        /// for the file upload
        /// </summary>
        /// <param name="image"></param>
        /// <returns>string</returns>
        public string FileUpload(FileModel image)
        {
            dbcontext.Files.Add(image);
              
            return "success";
        }
        /// <summary>
        /// to return the users by using id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>User</returns>
        // for user
       public User GetUsers(Guid id)
        {
            User user = dbcontext.Users.FirstOrDefault(o => o.Id == id);
           
            return user;
        }
        /// <summary>
        /// to return the address list
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //for address
        public List<Address> GetAddress(Guid id) { 
            List<Address> addresses = new List<Address>();
                   foreach(Address i in dbcontext.Addresses)
            {
                if(i.UserId==id)
                {
                    addresses.Add(i);
                }
            }
                   return addresses;
        }
        /// <summary>
        /// to return the email list
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //for email
        public List<Email> GetEmail(Guid id)
        {
            Email email = dbcontext.emails.FirstOrDefault(o => o.UserId == id);
            List<Email> email1 = new List<Email>();
            foreach (Email i in dbcontext.emails)
            {
                if (i.UserId == id)
                {
                    email1.Add(i);
                }
            }
            return email1;
        }
        /// <summary>
        /// to return the phone number table
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // for PhoneNumber
        public List<PhoneNumber> GetPhoneNumber(Guid id)
        {
            PhoneNumber phoneNumber = dbcontext.phoneNumbers.FirstOrDefault(o => o.UserId == id);
            List<PhoneNumber> phoneNumbers1 = new List<PhoneNumber>();
            foreach (PhoneNumber i in dbcontext.phoneNumbers)
            {
                if(i.UserId == id)  
                    phoneNumbers1.Add(i);
            }
            return phoneNumbers1;
        }

        /// <summary>
        /// to return the count of the address Book
        /// </summary>
        /// <returns></returns>
        // for count
        public int Count()
        {
            return dbcontext.Users.Count();
        }
        /// <summary>
        /// to find the Address Book by using id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // to find
        public Object Find(Guid id)
        {
           User result=dbcontext.Users.FirstOrDefault(o => o.Id == id);
            if (result == null)
                return null;
            if (result.IsActive == false)
                return null;
            return result;
        }
       /// <summary>
       /// to save the changes
       /// </summary>
        //to save
        public void Save()
        {
            dbcontext.SaveChanges();
        }
        /// <summary>
        /// to get the list of users
        /// </summary>
        /// <returns></returns>
        //get all the users
        public List<User> GetUsers()
        {
           List<User> users =dbcontext.Users.ToList();
            List<User> users1 = new List<User>();
           return users;
        }
        /// <summary>
        /// for the filters
        /// </summary>
        /// <param name="pagination"></param>
        /// <returns></returns>
        public List<User> GetUsers1(Pagination pagination)
        {
            var Collection = dbcontext.Users as IQueryable<User>;
            if (pagination.SortBy=="FirstName")
                Collection = Collection.OrderBy(x => x.FirstName);
            if (pagination.SortBy == "LastName")
                Collection = Collection.OrderBy(x => x.LastName);
            if(pagination.SortOrder =="DSC" && pagination.SortBy == "FirstName")
              Collection =Collection.OrderByDescending(x => x.FirstName);
            if (pagination.SortOrder == "DSC" && pagination.SortBy == "LastName")
                Collection = Collection.OrderByDescending(x => x.LastName);
            return Collection.Skip(pagination.pageSize * (pagination.pageNumber - 1)).Take(pagination.pageSize).ToList();
        }
        public RefSet Match(string s)
        {
            RefSet result = dbcontext.refSets.FirstOrDefault(o => o.Name.Equals(s));
            if (result == null)
                return null;
            return result;

        }
        public RefSet Match2(Guid guid)
        {
            var result = dbcontext.refSets.FirstOrDefault(o => o.RefSetId.Equals(guid));
            if (result == null)
                return null;
            return result;
        }
        public void Update(User user)
        {
           dbcontext.Users.Update(user);
           
        }
        public List<Email> GetEmail()
        {
            return dbcontext.emails.ToList();
        }
       
        /// <summary>
        /// get the file
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public Object GetFile(Guid guid)
        {
             FileModel result=dbcontext.Files.FirstOrDefault(o => o.Id.Equals(guid));
            if (result == null)
                return null;
            return result;

        }
        public metadata Findmeta(int id)
        {
           var meta=dbcontext.metadatas.FirstOrDefault(o=>o.Id==id);
            return meta;
        }
    }
}
