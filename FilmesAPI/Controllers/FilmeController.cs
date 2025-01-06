using AutoMapper;
using FilmesAPI.Data;
using FilmesAPI.Data.Dtos;
using FilmesAPI.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace FilmesAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class FilmeController : Controller
{

    private FilmeContext _context;
    private IMapper _mapper;

    public FilmeController(FilmeContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// Adiciona um filme ao banco de dados
    /// </summary>
    /// <param name="filmeDto">Objeto com os campos necessários para criação de um filme</param>
    /// <returns>IActionResult</returns>
    /// <response code="201">Caso inserção seja feita com sucesso</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public IActionResult AdicionaFilme([FromBody] CreateFilmeDto filmeDto)
    {
        Filme filme = _mapper.Map<Filme>(filmeDto);
        _context.Filmes.Add(filme);
        _context.SaveChanges();

        return CreatedAtAction(nameof(RecuperaFilmePorId), new { id = filme.Id }, filme);
    }

    /// <summary>
    /// Recupera uma lista de filmes, com controle de paginação
    /// </summary>
    /// <param name="skip">Número de registros a serem ignorados (página inicial)</param>
    /// <param name="take">Número de registros a serem retornados (quantidade por página)</param>
    /// <returns>Uma lista de filmes</returns>
    [HttpGet]
    public IEnumerable<ReadFilmeDto> RecuperaFilmes([FromQuery] int skip = 0,[FromQuery] int take = 50)
    {
        return _mapper.Map<List<ReadFilmeDto>>(_context.Filmes.Skip(skip).Take(take));
    }

    /// <summary>
    /// Recupera um filme específico pelo ID
    /// </summary>
    /// <param name="id">ID do filme a ser recuperado</param>
    /// <returns>O filme com o ID especificado ou um erro 404 caso não encontrado</returns>
    [HttpGet("{id}")]
    public IActionResult RecuperaFilmePorId(int id)
    {
        var filme = _context.Filmes.FirstOrDefault(f => f.Id == id);

        if (filme == null) return NotFound();

        var filmeDto = _mapper.Map<ReadFilmeDto>(filme);
        return Ok(filmeDto);
    }

    /// <summary>
    /// Atualiza completamente um filme existente
    /// </summary>
    /// <param name="id">ID do filme a ser atualizado</param>
    /// <param name="filmeDto">Objeto com os dados atualizados do filme</param>
    /// <returns>IActionResult</returns>
    /// <response code="204">Caso a atualização seja realizada com sucesso</response>
    /// <response code="404">Caso o filme não seja encontrado</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult AtualizaFilme(int id, [FromBody]UpdateFilmeDto filmeDto)
    {
        var filme = _context.Filmes.FirstOrDefault(f => f.Id == id);

        if (filme == null) return NotFound();

        _mapper.Map(filmeDto, filme);
        _context.SaveChanges();

        return NoContent();

    }

    /// <summary>
    /// Atualiza parcialmente um filme existente
    /// </summary>
    /// <param name="id">ID do filme a ser atualizado</param>
    /// <param name="patch">Objeto de patch contendo as atualizações parciais</param>
    /// <returns>IActionResult</returns>
    /// <response code="204">Caso a atualização parcial seja realizada com sucesso</response>
    /// <response code="404">Caso o filme não seja encontrado</response>
    /// <response code="400">Caso os dados fornecidos não sejam válidos</response>
    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult AtualizaFilmeParcial(int id, JsonPatchDocument<UpdateFilmeDto> patch)
    {
        var filme = _context.Filmes.FirstOrDefault(f => f.Id == id);

        if (filme == null) return NotFound();

        var filmeParaAtualizar = _mapper.Map<UpdateFilmeDto>(filme);

        patch.ApplyTo(filmeParaAtualizar, ModelState);

        if (!TryValidateModel(filmeParaAtualizar))
        {
            return ValidationProblem(ModelState);
        }

        _mapper.Map(filmeParaAtualizar, filme);
        _context.SaveChanges();

        return NoContent();

    }

    /// <summary>
    /// Deleta um filme do banco de dados
    /// </summary>
    /// <param name="id">ID do filme a ser deletado</param>
    /// <returns>IActionResult</returns>
    /// <response code="204">Caso a deleção seja realizada com sucesso</response>
    /// <response code="404">Caso o filme não seja encontrado</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult DeletaFilme(int id)
    {
        var filme = _context.Filmes.FirstOrDefault(f => f.Id == id);
        if (filme == null) return NotFound();

        _context.Filmes.Remove(filme);
        _context.SaveChanges();
        return NoContent();
    }

}
