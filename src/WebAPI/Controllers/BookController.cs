using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTO;

namespace BookManagement.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookController : ControllerBase
{
    private readonly IRepository<Book> _repository;

    public BookController(IRepository<Book> repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var books = await _repository.GetAllAsync();
        return Ok(books);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var book = await _repository.GetByIdAsync(id);
        if (book == null)
            return NotFound();

        return Ok(book);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] BookCreateDto dto)
    {
        var book = new Book(dto.Title, dto.Author, dto.Description);

        await _repository.InsertAsync(book);

        return CreatedAtAction(nameof(GetById), new { id = book.Id }, book);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] BookUpdateDto dto)
    {
        var book = await _repository.GetByIdAsync(id);
        if (book == null)
            return NotFound();

        book.Update(dto.Title, dto.Author, dto.Description);

        await _repository.UpdateAsync(book);

        return NoContent();
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _repository.DeleteAsync(id);
        return NoContent();
    }
}