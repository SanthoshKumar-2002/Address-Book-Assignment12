using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics.Metrics;
using System.Net;
using System.Security.Claims;
using System.Web.Http;
using WebApi.Entities.DTO;
using WebApi.Entities.Models;
using WebApi2;
using WebApi2.Contracts;
using WebApi2.Entities.DTO;
using WebApi2.Entities.Models;

namespace WebApi.Contracts
{

    public class UserService : IUserService
    {
       private readonly IUserRep userRep;
        private readonly ILogger<UserService> logger;
        public UserService(IUserRep userRep, ILogger<UserService> logger)
        {
            this.userRep = userRep;
            this.logger = logger;
        }
        /// <summary>
        /// for creating the new address book
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        //create
        public string Create(UserDTO user)
       {
            if (Usercheck(user))
            {
                string then = "exist";
                return then;
            }
           
            User object1 = new User();
             object1.Id = new Guid();
            object1.UserName = user.UserName;
            object1.Password = user.Password;    
             object1.FirstName = user.FirstName;
             object1.LastName = user.LastName;
            object1.CreatedOn = DateTime.Now;
            object1.CreatedBy = user.UserName;
                    object1.IsActive = true;
            List<Address> addresses = new List<Address>();
             for (int i = 0; i < user.Address.Count; i++)
             {
                 Address address1 = new Address()
                 {
                     UserId=object1.Id,
                     AddressId=new Guid(),
                     Line1 = user.Address[i].Line1,
                     Line2 = user. Address[i].Line2,
                     City = user. Address[i].City,
                     ZipCode = user.Address[i].ZipCode,
                     State = user.Address[i].stateName,
                     Type = (Guid)Type(user.Address[i].Type.Key),
                     country = (Guid)Type(user.Address[i].country.Key),
                     CreatedOn = DateTime.Now,
                     CreatedBy = user.UserName,
                     IsActive = true
                 };
                if (address1.Type == null)
                    return "Null";
                addresses.Add(address1);
             }
            object1.Address = addresses;
            List<Email> email = new List<Email>();
            for (int i = 0; i < user.Email.Count; i++)
            {
                if (emailcheck(user.Email[i].EmailAddress))
                    return "Email exist";
                    Email email1 = new Email()
                {
                    UserId =object1.Id, 
                    EmailId=new Guid(),
                    EmailAddress = user.Email[i].EmailAddress,
                   Type = (Guid)Type(user.Email[i].Type.Key),
                    CreatedOn = DateTime.Now,
                    CreatedBy = user.UserName,
                    IsActive = true
                };
                if (email1.Type == null)
                    return "Null";
                email.Add(email1);
            }
            object1.Email=email;
            List<PhoneNumber> phone = new List<PhoneNumber>();
            for (int i = 0; i < user.phones.Count; i++)
            {
                PhoneNumber phone1 = new PhoneNumber()
                {
                    UserId = object1.Id,
                    phoneId = new Guid(),
                    phoneNo = user.phones[i].phoneNo,
                    Type = (Guid)Type(user.phones[i].Type.Key),
                    CreatedOn = DateTime.Now,
                    CreatedBy = user.UserName,
                    IsActive = true
                };
                if (phone1.Type == null)
                phone.Add(phone1);
            }
            object1.phoneNumber=phone;
               
             Guid res1=userRep.Create(object1);
            if (res1 == null)
                return "Null";
            userRep.Save();
            return res1.ToString();
       }
        /// <summary>
        /// for getting all the users
        /// </summary>
        /// <param name="pagination"></param>
        /// <returns></returns>
        //get all the users
        public IEnumerable<GetDTO> GetAll(Pagination pagination)
        {

            List<GetDTO> userDTOs = new List<GetDTO>();

            List<User> user = userRep.GetUsers1(pagination);
            for (int i = 0; i < user.Count; i++)
            {
                GetDTO object2 = new GetDTO();
                if (user[i].IsActive != true)
                    continue;
                object2.Id=user[i].Id;
                object2.FirstName = user[i].FirstName;
                object2.LastName = user[i].LastName;
                object2.Address = new List<AddressGetDTO>();
                object2.Email = new List<EmailGetDTO>();
                object2.phones = new List<PhoneNumberGetDTO>();
                List<Address> address = userRep.GetAddress(user[i].Id);
                for (int j = 0; j < address.Count; j++)
                {
                    AddressGetDTO address2 = new AddressGetDTO()
                    {
                        AddressId = address[j].AddressId,
                        Line1 = address[j].Line1,
                        Line2 = address[j].Line2,
                        stateName = address[j].State,
                        ZipCode = address[j].ZipCode,
                        City = address[j].City,
                        country= Type2(address[j].country),
                        Type = Type2(address[j].Type)
                    };
                    object2.Address.Add(address2);
                }
                List<Email> emails = userRep.GetEmail(user[i].Id);
                for (int j = 0; j < emails.Count; j++)
                {
                    EmailGetDTO emailDTO = new EmailGetDTO()
                    {
                        EmailId = emails[j].EmailId,
                        EmailAddress = emails[j].EmailAddress,
                        Type = Type2(emails[j].Type)
                    };
                    object2.Email.Add(emailDTO);
                }
                List<PhoneNumber> phoneNumbers = userRep.GetPhoneNumber(user[i].Id);
                for (int j = 0; j < phoneNumbers.Count; j++)
                {
                    PhoneNumberGetDTO phoneNumberDTO = new PhoneNumberGetDTO()
                    {
                        PhoneNumberId=phoneNumbers[j].phoneId,
                        phoneNo = phoneNumbers[j].phoneNo,
                        Type = Type2(phoneNumbers[j].Type)
                    };
                    object2.phones.Add(phoneNumberDTO);
                }
                userDTOs.Add(object2);
            }
            return userDTOs;
        }
        /// <summary>
        /// for getting the count
        /// </summary>
        /// <returns></returns>
        //Count
        public int GetCount()
        {
            int count = userRep.GetUsers().Count;
            logger.LogInformation("Count success");
            return count;
        }
        /// <summary>
        /// for getting the address Book by using id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>GetDTO</returns>
        // get by id
        public GetDTO GetId(Guid id)
        {
            GetDTO user = new GetDTO();
            User find = (User)userRep.Find(id);
            if (find == null)
                return null;
            user.Id = id;
            user.FirstName = find.FirstName;
            user.LastName = find.LastName;
            user.Address = new List<AddressGetDTO>();
            user.Email = new List<EmailGetDTO>();
            user.phones = new List<PhoneNumberGetDTO>();
            List<Address> addresses = userRep.GetAddress(id);

            for (int i = 0; i < addresses.Count; i++)
            {
                AddressGetDTO address1 = new AddressGetDTO()
                {
                    AddressId = addresses[i].AddressId,
                    Line1 = addresses[i].Line1,
                    Line2 = addresses[i].Line2,
                    City = addresses[i].City,
                    ZipCode = addresses[i].ZipCode,
                    stateName = addresses[i].State,
                    Type = Type2(addresses[i].Type),
                   country= Type2(addresses[i].country)
                };
                user.Address.Add(address1);

            }
            List<Email> email = userRep.GetEmail(id);
            for (int i = 0; i < email.Count; i++)
            {
                EmailGetDTO email1 = new EmailGetDTO()
                {
                    EmailId = email[i].EmailId,
                    EmailAddress = email[i].EmailAddress,
                    Type = Type2(email[i].Type)
                };
                user.Email.Add(email1);
            }
            List<PhoneNumber> phoneNumbers = userRep.GetPhoneNumber(id);
            for (int i = 0; i < phoneNumbers.Count; i++)
            {
                PhoneNumberGetDTO phoneNumber1 = new PhoneNumberGetDTO()
                {
                    PhoneNumberId = phoneNumbers[i].phoneId,
                    phoneNo = phoneNumbers[i].phoneNo,
                    Type = Type2(phoneNumbers[i].Type)
                };
                user.phones.Add(phoneNumber1);
            }
            if((bool)find.IsActive)
            return user;
            return null;

        }
        /// <summary>
        /// for updating the address book
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userin"></param>
        /// <returns></returns>
        //update
        public Object Update(Guid id, UserDTO userin)
        {
            User update = (User)userRep.Find(id);
            
          
            if (update == null)
            {
                return null;
            }
           
                update.UserName = userin.UserName;
                update.Password = userin.Password;
                update.FirstName = userin.FirstName;
                update.LastName = userin.LastName;
                update.UpdatedOn = DateTime.Now;
                update.UpdateBy = userin.UserName;
                update.Address = new List<Address>();
                update.Email = new List<Email>();
                update.phoneNumber = new List<PhoneNumber>();
                List<Address> addresses = new List<Address>();
                for (int i = 0; i < userin.Address.Count; i++)
                {
                    Address address1 = new Address()
                    {

                        Line1 = userin.Address[i].Line1,
                        Line2 = userin.Address[i].Line2,
                        City = userin.Address[i].City,
                        ZipCode = userin.Address[i].ZipCode,
                        State = userin.Address[i].stateName,
                        Type = (Guid)Type(userin.Address[i].Type.Key),
                        country = (Guid)Type(userin.Address[i].country.Key),
                         UpdatedOn = DateTime.Now,
                         UpdateBy = userin.UserName,
                         IsActive=true
                };
                if (address1.Type == null)
                    return null;
                addresses.Add(address1);

                }
                update.Address = addresses;
                List<Email> email = new List<Email>();
                for (int i = 0; i < userin.Email.Count; i++)
                {
                if (emailcheck(userin.Email[i].EmailAddress))
                    throw new Exception();
                Email email1 = new Email()
                    {

                        EmailAddress = userin.Email[i].EmailAddress,
                        Type = (Guid)Type(userin.Email[i].Type.Key),
                        UpdatedOn = DateTime.Now,
                        UpdateBy = userin.UserName,
                        IsActive = true
                    };
                if (email1.Type == null)
                    return null;
                email.Add(email1);
                }
            
            update.Email = email;
                List<PhoneNumber> phone = new List<PhoneNumber>();
                for (int i = 0; i < userin.phones.Count; i++)
                {
                    PhoneNumber phone1 = new PhoneNumber()
                    {

                        phoneNo = userin.phones[i].phoneNo,
                        Type = (Guid)Type(userin.phones[i].Type.Key),
                        UpdatedOn = DateTime.Now,
                        UpdateBy = userin.UserName,
                        IsActive = true
                    };
                if (phone1.Type == null)
                    return null;
                phone.Add(phone1);
                }
                update.phoneNumber = phone;
                userRep.Update(update);
               userRep.Save();
                UpdateDTO user = new UpdateDTO();
                user.Id=update.Id;
                user.UserName = update.UserName;
                user.FirstName = update.FirstName;
                user.LastName = update.LastName;
                user.Address = new List<AddressDTO>();
                user.Email = new List<EmailDTO>();
                user.phoneNumber = new List<PhoneNumberDTO>();
                List<Address> addresseslist = userRep.GetAddress(id);

                for (int i = 0; i < addresses.Count; i++)
                {
                    AddressDTO address1 = new AddressDTO()
                    {

                        Line1 = addresseslist[i].Line1,
                        Line2 = addresseslist[i].Line2,
                        City = addresseslist[i].City,
                        ZipCode = addresseslist[i].ZipCode,
                        stateName = addresseslist[i].State,
                        Type = Type2(addresseslist[i].Type),
                        country= Type2(addresseslist[i].country )
                    };
                    user.Address.Add(address1);

                }
                List<Email> emailList = userRep.GetEmail(id);
                for (int i = 0; i < email.Count; i++)
                {
                   
                    EmailDTO email1 = new EmailDTO()
                    {
                        EmailAddress = emailList[i].EmailAddress,
                        Type = Type2(emailList[i].Type)
                    };
                    user.Email.Add(email1);
                }
                List<PhoneNumber> phoneNumbers = userRep.GetPhoneNumber(id);
                for (int i = 0; i < phoneNumbers.Count; i++)
                {
                    PhoneNumberDTO phoneNumber1 = new PhoneNumberDTO()
                    {
                        phoneNo = phoneNumbers[i].phoneNo,
                        Type = Type2(phoneNumbers[i].Type)
                    };
                    user.phoneNumber.Add(phoneNumber1);
                }
            logger.LogInformation("update id success");
            return user;
        }
        /// <summary>
        /// for deleting the address book
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //delete
        public Object Delete(Guid id)
        {
            bool delete = userRep.Delete(id);
            
            userRep.Save();
            string result = "Address book deleted successfully";
            if (delete)
            {
                logger.LogInformation("Delete success");
                return result;
            }
            return null;

        }
        /// <summary>
        /// for uploading the file
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        //file upload
        public FilesDTO FileUpload(FileModelDTO image)
        {
            if (image == null)
                throw new Exception();
            IFormFile file = image.FormFile;
            FileModel fileModel = new FileModel();
            fileModel.Id = new Guid();
            fileModel.FileName=image.FormFile.FileName;
            fileModel.FileType = image.FormFile.ContentType;
            fileModel.Size = image.FormFile.Length;
             byte[] GetBytes(IFormFile formFile)
            {
                 using MemoryStream memoryStream = new MemoryStream();
                 formFile.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
            byte[] bytes = GetBytes(file);

            fileModel.FormFile = bytes;

            string res=userRep.FileUpload(fileModel);
            userRep.Save();
            FilesDTO filesDTO = new FilesDTO()
            {
                Id = fileModel.Id,
                FileName = fileModel.FileName,
                DownloadUrl = "https://localhost:7219/api/asset/downloadFile/" + fileModel.Id,
                FileType = fileModel.FileType,
                Size = fileModel.Size,

            };

            logger.LogInformation("File uploaded successfully");
            return filesDTO;
        }
        /// <summary>
        /// return guid id in ref table by using value
        /// </summary>
        /// <param name="type1"></param>
        /// <returns></returns>
        //convert string to guid

        public Object Type(string type1)
        {
            string sub = type1;
            RefSet guid = userRep.Match(sub);
            if (guid == null)
               throw new KeyNotFoundException();
            
                Guid guid1 = guid.RefSetId;
            logger.LogInformation("changed from string to guid");
            return guid1;
        }
        /// <summary>
        /// returns the type by using guid
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        //convert guid to string
        public TypesDTO Type2(Guid guid)
        {
            RefSet s = userRep.Match2(guid);
            if (s == null)
                return null;

            string guid1 = s.Name;
            TypesDTO typesDTO = new TypesDTO()
            {
                Key = guid1
            };
           logger.LogInformation("guid to string change success");
            return typesDTO;  
        }

       /// <summary>
       /// to check the user exist or not
       /// </summary>
       /// <param name="user"></param>
       /// <returns></returns>
        public bool Usercheck(UserDTO user)
        {
            List<User> check1 = userRep.GetUsers();
            if (check1.Count() == 0)
                return false;
            User find;
                find = check1.FirstOrDefault(o => o.UserName.Equals(user.UserName));
            if (find != null)
            {
                return true;
            }
            return false;

        }
        /// <summary>
        /// to check the user by user
        /// </summary>
        /// <param name="user"></param>
        /// <returns>bool</returns>
        public bool usercheck(string user)
        {
            List<User> check1 = userRep.GetUsers();

            User find;
            find = check1.FirstOrDefault(o => o.UserName.Equals(user));
            if (find == null)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// for authorization
        /// </summary>
        /// <param name="claimsIdentity"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        public bool authorize(ClaimsIdentity claimsIdentity, Guid id)
        {
            var userclaims = claimsIdentity.Claims;
            string result = userclaims.FirstOrDefault(o => o.Type == ClaimTypes.NameIdentifier).Value;
          
            List<User> user = userRep.GetUsers();
            User find = user.FirstOrDefault(o => o.UserName.Equals(result));
            if (find == null)
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            return true;
        }
        /// <summary>
        /// to get the file from the database
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public Object GetFile(Guid guid)
        {
            var res = userRep.GetFile(guid);
            if (res == null)
                return null;
            return res;
        }
        /// <summary>
        /// to get the meta data
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public metaDataDTO GetMetadata(int id)
        {
           metadata user= userRep.Findmeta(id);
            metaDataDTO result = new metaDataDTO();

            result.Id = user.Id;
            result.key = user.key;
            result.description = user.description;
            result.list = new List<string>()
            { "WORK", "PERSONAL", "ALTERNATE" };



            logger.LogInformation("meta-data get success");
            return result;
        }
        public bool emailcheck(string email)
        {
            List < Email > list= userRep.GetEmail();
            var result=list.FirstOrDefault(o=>o.EmailAddress==email);
            if(result==null)
                return false;
            return true;
        }
    }
}
