using System;
using System.Collections.Generic;
using SmartRoutes.Model.Odjfs.ChildCareStubs;

namespace SmartRoutes.OdjfsScraper.Test.Parsers.Support
{
    public class ListTemplate : BaseTemplate, ITemplate<IList<ChildCareStub>>
    {
        public ListTemplate()
        {
            Model = new List<ChildCareStub>();
        }

        public IList<ChildCareStub> Model { get; private set; }

        public byte[] GetDocument()
        {
            throw new NotImplementedException();
        }
    }
}