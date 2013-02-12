﻿#region

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using ApprovalTests;
using ApprovalTests.Reporters;
using WebApi.Hal.Tests.Representations;
using Xunit;

#endregion

namespace WebApi.Hal.Tests
{
    public class HalResourceTypedTest
    {
        readonly OrganisationRepresentationTyped resource;

        public HalResourceTypedTest()
        {
            var model = new Organisation(1, "Org Name");
            var emps = new List<Employee>
            {
                new Employee(1, "emp1", 1),
                new Employee(11, "emp2", 2),
                new Employee(12, "emp1", 4),
            };

            var employeeRepresentations = emps.Select(e => new EmployeeRepresentation(e)).ToList();
            model.Employees = new EmployeeRepresentationList(employeeRepresentations);
            resource = new OrganisationRepresentationTyped(model);
        }

        [Fact]
        [UseReporter(typeof (DiffReporter))]
        public void organisation_get_json_test()
        {
            // arrange
            var mediaFormatter = new JsonHalMediaTypeFormatter {Indent = true};
            var content = new StringContent(string.Empty);
            var type = resource.GetType();

            // act
            using (var stream = new MemoryStream())
            {
                mediaFormatter.WriteToStreamAsync(type, resource, stream, content, null);
                stream.Seek(0, SeekOrigin.Begin);
                var serialisedResult = new StreamReader(stream).ReadToEnd();

                // assert
                Approvals.Verify(serialisedResult);
            }
        }

        [Fact]
        [UseReporter(typeof (DiffReporter))]
        public void organisation_get_xml_test()
        {
            // arrange
            var mediaFormatter = new XmlHalMediaTypeFormatter();
            var content = new StringContent(string.Empty);
            var type = resource.GetType();

            // act
            using (var stream = new MemoryStream())
            {
                mediaFormatter.WriteToStreamAsync(type, resource, stream, content, null);
                stream.Seek(0, SeekOrigin.Begin);
                var serialisedResult = new StreamReader(stream).ReadToEnd();

                // assert
                Approvals.Verify(serialisedResult);
            }
        }
    }
}