using Xunit;
using Notes.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace Notes.Tests
{
    public class IntegrationTest
    {
        private static string _connectionString = "Server=(localdb)\\mssqllocaldb;Database=NotesContext-233a2f94-4597-40be-8b62-dd6f481e1178;Trusted_Connection=True;MultipleActiveResultSets=true";
        private static DbContextOptionsBuilder<NotesContext> _optionsBuilder = new DbContextOptionsBuilder<NotesContext>().UseSqlServer(_connectionString);
        private static NotesContext _context = new NotesContext(_optionsBuilder.Options);
        private static NotesController _controller = new NotesController(_context);

        private void ClearDB()
        {
            var allNotes = _controller.GetNote();
            System.Threading.Tasks.Task<IActionResult> tmp;
            foreach (Note n in allNotes)
                tmp = _controller.DeleteNote(n.ID);
        }

        [Fact]
        public void TestGET()
        {
            ClearDB();
            Note tmpNote0 = new Note();
            Note tmpNote1 = new Note();
            tmpNote0.Text = "TestnoteGET";
            tmpNote1.Text = "TestnoteGET1";

            var res0 = _controller.PostNote(tmpNote0);
            var res1 = _controller.PostNote(tmpNote1);

            var allNotes = _controller.GetNote();
            int count = 0;
            foreach (Note n in allNotes)
            {
                if (count == 0)
                    Assert.Equal(tmpNote0.Text, n.Text);
                if (count == 1)
                    Assert.Equal(tmpNote1.Text, n.Text);
                count++;
            }
            ClearDB();
        }

        [Fact]
        public void TestPOST()
        {
            ClearDB();
            Note tmpNote = new Note();
            tmpNote.Text = "TestnotePOST";

            var actionResult = _controller.PostNote(tmpNote);
            var createdAtActionResult = actionResult.Result as CreatedAtActionResult;
            var note = createdAtActionResult.Value as Note;

            var actionResultGet = _controller.GetNote(note.ID);
            var okObjectResult = actionResultGet.Result as OkObjectResult;
            note = okObjectResult.Value as Note;

            Assert.Equal(tmpNote.Text, note.Text);
            ClearDB();
        }

        [Fact]
        public void TestPUT()
        {
            ClearDB();
            Note tmpNote = new Note();
            tmpNote.Text = "TestnotePUTOriginal";
            var actionResult0 = _controller.PostNote(tmpNote);
            var createdAtActionResult = actionResult0.Result as CreatedAtActionResult;
            var oritinalNote = createdAtActionResult.Value as Note;

            Note modifiedNote = new Note();
            modifiedNote.Text = "TestnotePUTModified";
            modifiedNote.ID = oritinalNote.ID;
            var actionResult1 = _controller.PutNote(oritinalNote.ID, modifiedNote);

            _context.Entry(tmpNote).Reload();

            var actionResult2 = _controller.GetNote(oritinalNote.ID);
            var okObjectResult = actionResult2.Result as OkObjectResult;
            var modifiedNoteFromGet = okObjectResult.Value as Note;
            
            Assert.Equal(modifiedNote.Text, modifiedNoteFromGet.Text);
            ClearDB();
        }
        
        [Fact]
        public void TestDelete()
        {
            ClearDB();
            Note tmpNote = new Note();
            tmpNote.Text = "TestnoteDELETE";
            var actionResult0 = _controller.PostNote(tmpNote);
            var createdAtActionResult = actionResult0.Result as CreatedAtActionResult;
            var note = createdAtActionResult.Value as Note;

            var actionResult1 = _controller.GetNote(note.ID);
            var result = actionResult1.Result as OkObjectResult;
            var noteFromGet = result.Value as Note;

            Assert.Equal(tmpNote.Text, noteFromGet.Text);

            var actionResult2 = _controller.DeleteNote(noteFromGet.ID);
            var actionResult3 = _controller.GetNote(noteFromGet.ID);
            var notFoundResult = actionResult3.Result as NotFoundResult;

            Assert.Equal(404, notFoundResult.StatusCode);
            ClearDB();
        }
    }
}
