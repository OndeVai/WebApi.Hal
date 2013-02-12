#region

using System.Collections.Generic;
using Newtonsoft.Json;

#endregion

namespace WebApi.Hal.Tests.Representations
{
    public class OrganisationListRepresentationListTyped : RepresentationList<OrganisationRepresentationTyped>
    {
        public OrganisationListRepresentationListTyped(
            IList<OrganisationRepresentationTyped> organisationRepresentations) :
                base(organisationRepresentations)
        {
        }

        public int Count { get; set; }

        protected override void CreateHypermedia()
        {
            Rel = "organisations";
            Href = "/api/organisations";
        }
    }

    public class OrganisationRepresentationTyped : Representation<Organisation>
    {
        public OrganisationRepresentationTyped()
        {
        }

        public OrganisationRepresentationTyped(Organisation model)
        {
            Model = model;
        }

        protected override void CreateHypermedia()
        {
            Rel = "organisation";
            Href = string.Format("/api/organisations/{0}", Model.Id);

            Links.Add(new Link("related", "/api/other"));
        }
    }

    public class Organisation
    {
        public Organisation()
        {
        }

        public Organisation(int id, string name)
            : this()
        {
            Id = id;
            Name = name;
        }


        public int Id { get; set; }
        public string Name { get; set; }
        public EmployeeRepresentationList Employees { get; set; }

        [JsonIgnore]
        public string ToIgnore { get; set; }
    }

    public class EmployeeRepresentationList : RepresentationList<EmployeeRepresentation>
    {
        public EmployeeRepresentationList(IList<EmployeeRepresentation> res) : base(res)
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