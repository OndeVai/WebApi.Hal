#region

using System.Collections.Generic;
using Newtonsoft.Json;

#endregion

namespace WebApi.Hal.Tests.Representations
{


    public class EmployeeRepresentationList : RepresentationList<EmployeeRepresentation>
    {
        public EmployeeRepresentationList(IList<EmployeeRepresentation> res)
            : base(res)
        {
        }

        protected override void CreateHypermedia()
        {
            Rel = "employees";
            Href = "/api/employees";
        }
    }

    public class EmployeeRepresentation : Representation<Employee>
    {
        public EmployeeRepresentation()
        {
        }

        public EmployeeRepresentation(Employee model)
        {
            Model = model;
        }

        protected override void CreateHypermedia()
        {
            Href = "/api/employees/" + Model.Id;
            LinkTitle = "see employee " + Model.Title;

            Links.Add(new Link("related", "sometoher").UpdateTitle("see related"));
        }
    }

    public class Employee
    {
        public Employee()
        {
        }

        public Employee(int id, string title, int rank)
        {
            Id = id;
            Title = title;
            Rank = rank;
        }


        public int Id { get; set; }
        public string Title { get; set; }
        public int Rank { get; set; }

        [JsonIgnore]
        public string Ignore { get; set; }
    }
}