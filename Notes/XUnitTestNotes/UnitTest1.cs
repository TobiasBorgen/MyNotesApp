using System;
using Xunit;
using Notes.Controllers;
using Notes.Models;

namespace XUnitTestNotes
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var controller = new NotesController();
        }
    }
}
