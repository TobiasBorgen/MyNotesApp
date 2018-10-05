using System;
using Xunit;
using Notes;
using Notes.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Notes.Tests
{
    public class UnitTest1
    {

        private static string connectionString = "Server=(localdb)\\mssqllocaldb;Database=NotesContext-233a2f94-4597-40be-8b62-dd6f481e1178;Trusted_Connection=True;MultipleActiveResultSets=true";
        private static DbContextOptionsBuilder<NotesContext> optionsBuilder = new DbContextOptionsBuilder<NotesContext>().UseSqlServer(connectionString);
        private static NotesContext context = new NotesContext(optionsBuilder.Options);
        private static NotesController controller = new NotesController(context);

        private void ClearDB()
        {
            var AllNotes = controller.GetNote();
            System.Threading.Tasks.Task<IActionResult> tmp;
            foreach (Note n in AllNotes)
                tmp = controller.DeleteNote(n.ID);
        }

        [Fact]
        public void TestGET()
        {
            ClearDB();
            Note my_note = new Note();
            my_note.Text = "TestnoteGET";
            Note my_note1 = new Note();
            my_note1.Text = "TestnoteGET1";
            var actionResult = controller.PostNote(my_note);

            var ret_task = controller.GetNote();
            int count = 0;
            foreach (Note n in ret_task)
            {
                if (count == 0)
                    Assert.Equal(my_note.Text, n.Text);
                //if (count == 1)
                //    Assert.Equal(my_note1.Text, n.Text);

                //Debug.WriteLine(n.Text + n.ID);
                count++;
            }
        }

        [Fact]
        public void TestPOST()
        {
            ClearDB();
            Note my_note = new Note();
            my_note.Text = "TestnotePOST";
            var actionResult = controller.PostNote(my_note);
            var asdf = actionResult.Result as CreatedAtActionResult;
            var note = asdf.Value as Note;
            //Debug.WriteLine(note.Text + note.ID);

            var ret_task = controller.GetNote(note.ID);
            var result = ret_task.Result as OkObjectResult;
            note = result.Value as Note;
            //Debug.WriteLine(note.Text + note.ID);

            Assert.Equal(my_note.Text, note.Text);
        }

        [Fact]
        public void TestPUT()
        {
            ClearDB();

            Note my_note = new Note();
            my_note.Text = "TestnotePUTOriginal";
            var actionResult = controller.PostNote(my_note);
            var asdf = actionResult.Result as CreatedAtActionResult;
            var note = asdf.Value as Note;

            Note my_modified_note = new Note();
            my_modified_note.Text = "TestnotePUTModified";
            my_modified_note.ID = note.ID;
            var actionResult2 = controller.PutNote(note.ID, my_modified_note);

            //optionsBuilder = new DbContextOptionsBuilder<NotesContext>().UseSqlServer(connectionString);
            //context = new NotesContext(optionsBuilder.Options);
            //controller = new NotesController(context);
            context.Entry(my_note).Reload();

            var ret_task = controller.GetNote(note.ID);
            var result = ret_task.Result as OkObjectResult;
            var note1 = result.Value as Note;

            var all = controller.GetNote();
            foreach (Note n in all)
            {
                Debug.WriteLine(n.Text + n.ID);
            }
            //Debug.WriteLine(note.Text + note.ID);
            
            Assert.Equal(my_modified_note.Text, note1.Text);

        }
        
        [Fact]
        public void TestDelete()
        {
            //ClearDB();

        }

    }
}
