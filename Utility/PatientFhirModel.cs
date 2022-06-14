namespace eIVF.Utility
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Address
    {
        public string use { get; set; }
        public string type { get; set; }
        public string text { get; set; }
        public List<string> line { get; set; }
        public string city { get; set; }
        public string district { get; set; }
        public string state { get; set; }
        public string postalCode { get; set; }
        public Period period { get; set; }
    }

    public class Assigner
    {
        public string display { get; set; }
    }

    public class BirthDate
    {
        public List<Extension> extension { get; set; }
    }

    public class Coding
    {
        public string system { get; set; }
        public string code { get; set; }
    }

    public class Contact
    {
        public List<Relationship> relationship { get; set; }
        public Name name { get; set; }
        public List<Telecom> telecom { get; set; }
        public Address address { get; set; }
        public string gender { get; set; }
        public Period period { get; set; }
    }

    public class Extension
    {
        public string url { get; set; }
        public DateTime valueDateTime { get; set; }
        public string valueString { get; set; }
    }

    public class Family
    {
        public List<Extension> extension { get; set; }
    }

    public class Identifier
    {
        public string use { get; set; }
        public Type type { get; set; }
        public string system { get; set; }
        public string value { get; set; }
        public Period period { get; set; }
        public Assigner assigner { get; set; }
    }

    public class ManagingOrganization
    {
        public string reference { get; set; }
    }

    public class Meta
    {
        public string versionId { get; set; }
        public DateTime lastUpdated { get; set; }
    }

    public class Name
    {
        public string use { get; set; }
        public string family { get; set; }
        public List<string> given { get; set; }
        public Period period { get; set; }
        public Family _family { get; set; }
    }

    public class Period
    {
        public string start { get; set; }
        public string end { get; set; }
    }

    public class Relationship
    {
        public List<Coding> coding { get; set; }
    }

    public class PatientFhirModel
    {
        public string resourceType { get; set; }
        public string id { get; set; }
        public Meta meta { get; set; }
        public List<Identifier> identifier { get; set; }
        public bool active { get; set; }
        public List<Name> name { get; set; }
        public List<Telecom> telecom { get; set; }
        public string gender { get; set; }
        public string birthDate { get; set; }
        public BirthDate _birthDate { get; set; }
        public bool deceasedBoolean { get; set; }
        public List<Address> address { get; set; }
        public List<Contact> contact { get; set; }
        public ManagingOrganization managingOrganization { get; set; }
    }

    public class Telecom
    {
        public string use { get; set; }
        public string system { get; set; }
        public string value { get; set; }
        public int? rank { get; set; }
        public Period period { get; set; }
    }

    public class Type
    {
        public List<Coding> coding { get; set; }
    }


}
