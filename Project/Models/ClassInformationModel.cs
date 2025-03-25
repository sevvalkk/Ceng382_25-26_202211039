namespace Project.Models
{
    public class ClassInformationModel
    {
        private static int counter = 1;

        public int Id { get;  set; }
        public string ClassName { get; set; }
        public int StudentCount { get; set; }
        public string Description { get; set; }

        public ClassInformationModel()
        {
            Id = counter++;
        }

        public ClassInformationModel(string className, int studentCount, string description) : this()
        {
            ClassName = className;
            StudentCount = studentCount;
            Description = description;
        }
    }
}
