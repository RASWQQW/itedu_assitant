using Aspose.Email.Clients.Smtp;
using itedu_assitant.DB;
using itedu_assitant.forsave.Methods;
using itedu_assitant.forsave.Serializer;
using itedu_assitant.Model.Base;
using itedu_assitant.Model.ForvView;
using Microsoft.AspNetCore.DataProtection;
using Newtonsoft.Json;
using System;
using System.Security.Cryptography;

namespace itedu_assitant.forsave.Contact_is.Methods
{
    public class AuthCodeGetting
    {
        public CreateContact Creater;
        public AuthCodeGetting(CreateContact creater)
        {
            Creater = creater;  
        }

        public Tuple<bool, string> ExecuteCreatingContact(ContactMembers contactMembers = null, bool byQueue = false)
        {
            var objectIs = Creater.SetCredential();
            if (objectIs.content.Item1)
            {
                if (contactMembers == null && byQueue)
                {
                    var enval = Environment.GetEnvironmentVariable("ContactCreateTask");
                    if (enval == null || enval == "false")
                    {
                        // Here goes first run if here is not active thread
                        BackgroundTaskManage isback = new BackgroundTaskManage(QueueExecuting, new Dictionary<string, object> { { "execobjec", objectIs } });
                        CancellationTokenSource nct = new CancellationTokenSource();
                        isback.StartAsync(nct.Token);
                    }
                }
                else
                    // if here all are ready here just goes running method
                    CommonRunner(contactMembers, objectIs);

                string watchstring = "https://contacts.google.com/";
                return new Tuple<bool, string>(true, $"Contacts Created You can see it in {watchstring}");
            }
            else
            {
                // here is only one time, in contact manager will set values when it only presence
                if (byQueue == false && contactMembers != null)
                    SetToQueue(contactMembers);

                // and lasly returns failure value to redirect specefic url
                return new Tuple<bool, string>(false, objectIs.content.Item2);
            }
        }

        public static void CommonRunner(ContactMembers contactMembers, CreateContact objectIs)
        {
            if (contactMembers.contacts != null)
            {
                List<Dictionary<string, string>> values = new List<Dictionary<string, string>>();
                foreach (Contact contact in contactMembers.contacts)
                {
                    values.Add(new Dictionary<string, string> { { "ContactName", contact.ContactName }, { "PhoneNumber", contact.ContactPhoneNumber } });
                }
                objectIs.Number(values);
            }
            else if (contactMembers.groupId != null)
                objectIs.Number(contactMembers.groupId.ToString());
        }

        // It has his own method to make either with queue or model one time
        public static void QueueExecuting(object? state)
        {
            if (state.GetType() == typeof(Dictionary<,>))
            {
                Dictionary<string, object> istate = (Dictionary<string, object>)state;
                CreateContact objectIs = (CreateContact)istate["execobject"];

                // to set that thread actually working and no needs to run it 
                Environment.SetEnvironmentVariable("ContactCreateTask", "true");

                while (true)
                {
                    var isval = GetQueue();
                    if (isval.GetType() == typeof(ContactMembers))
                    {
                        ContactMembers contactMembers = (ContactMembers)isval;
                        CommonRunner(contactMembers, objectIs);
                    }
                    else
                    {
                        // setting last end acception
                        Environment.SetEnvironmentVariable("ContactCreateTask", "false");
                        break;
                    }
                }
            }
        }

        // TODO one check of queue save please
        public void SetToQueue(ContactMembers values)
        {
            // Actually it runs when here goes values of first step failure
            QueueSer iserializer = new QueueSer();
            var isserobj = iserializer.DeSerialize();

            if (!(bool)isserobj)
                isserobj = new Queue<string>();
            ((Queue<string>)isserobj).Enqueue(JsonConvert.SerializeObject(values));
            iserializer.Serialize(isserobj);
        }
        static public object GetQueue()
        {
            QueueSer iser = new QueueSer();
            var stackobject = iser.DeSerialize();
            if (!(bool)stackobject)
            {
                var isqueue = (Queue<string>)stackobject;
                if (isqueue.Count() > 0)
                {
                    string isfirsline = isqueue.Dequeue();
                    return JsonConvert.DeserializeObject<ContactMembers>(isfirsline);
                }
                else return false;
            }
            throw new Exception("Here is not any values of it");
        }
    }
}
