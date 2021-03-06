#region

using System.Collections.Generic;

#endregion

namespace WebApi.Hal.Tests.Representations
{
    public class OrganisationListRepresentation : RepresentationList<OrganisationRepresentation>
    {
        public OrganisationListRepresentation(IList<OrganisationRepresentation> organisationRepresentations) :
            base(organisationRepresentations)
        {
        }


        protected override void CreateHypermedia()
        {
            Rel = "organisations";
            Href = "/api/organisations";
        }
    }
}