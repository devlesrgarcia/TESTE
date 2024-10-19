using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using APIUsuario.Context;
using APIUsuario.Models;
using System.Collections.Generic;
using System.Linq;

namespace APIUsuario.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsuarioController : ControllerBase
    {
        // Crio uma variável estática para armazenar os dados do usuário
        private static dynamic _userData;
        // Crio o contexto do DB
        private readonly APIUsuarioContext _context;
        public UsuarioController(APIUsuarioContext context)
        {
            _context = context;
        }
        // Endpoint para obter todos os usuários
        [HttpGet("ObterTodos", Name = "ObterTodos")]
        public IActionResult Get()
        {
            try
            {
                var usuarios = _context.Usuarios.ToList();
                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter usuários: {ex.Message}");
            }
        }
        // Endpoint para API RANDOMUSER
        [HttpGet("APIRandom", Name = "APIRandom")]
        public async Task<IActionResult> GetRandom()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync("https://randomuser.me/api/");

                    if (response.IsSuccessStatusCode)
                    {
                        // Aqui eu converto para json
                        string content = await response.Content.ReadAsStringAsync();
                        JObject result = JObject.Parse(content);

                        // Extrai os dados
                        string name = result["results"][0]["name"]["first"].ToString() + " " +
                                      result["results"][0]["name"]["last"].ToString();
                        string phone = result["results"][0]["phone"].ToString();
                        string street = result["results"][0]["location"]["street"]["name"].ToString() + " N° " +
                                        result["results"][0]["location"]["street"]["number"].ToString();
                        string city = result["results"][0]["location"]["city"].ToString();
                        string state = result["results"][0]["location"]["state"].ToString();

                        // Armazeno dentro de _userData
                        _userData = new
                        {
                            Nome = name,
                            Telefone = phone,
                            Rua = street,
                            Cidade = city,
                            Estado = state
                        };

                        return Ok(_userData);
                    }
                    else
                    {
                        return StatusCode((int)response.StatusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro: {ex.Message}");
            }
        }

        // Endpoint para cadastrar usando os dados da API RANDOMUSER
        [HttpPost]
        public IActionResult Post()
        {
            try
            {
                if (_userData == null)
                {
                    return BadRequest("Execute primeiro o GET para obter os dados.");
                }

                // Cria um novo usuário com os dados
                var usuario = new Usuario
                {
                    Nome = _userData.Nome,
                    Telefone = _userData.Telefone,
                    Rua = _userData.Rua,
                    Cidade = _userData.Cidade,
                    Estado = _userData.Estado
                };

                _context.Usuarios.Add(usuario);
                _context.SaveChanges();

                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao criar usuário: {ex.Message}");
            }
        }

        // Endpoint para editar
        [HttpPut("{id}")]
        public IActionResult Put(int id, Usuario usuarioNovo)
        {
            try
            {
                // Verifica se o ID digitado existe
                var usuarioAtual = _context.Usuarios.FirstOrDefault(u => u.Usuarioid == id);

                if (usuarioAtual == null)
                {
                    return NotFound($"Usuário com ID {id} não encontrado.");
                }

                usuarioAtual.Nome = usuarioNovo.Nome;
                usuarioAtual.Telefone = usuarioNovo.Telefone;
                usuarioAtual.Rua = usuarioNovo.Rua;
                usuarioAtual.Cidade = usuarioNovo.Cidade;
                usuarioAtual.Estado = usuarioNovo.Estado;

                _context.SaveChanges();

                return Ok(usuarioAtual);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao atualizar usuário: {ex.Message}");
            }
        }

        // Endpoint para excluir 
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                // Verifica se o ID digitado existe
                var usuario = _context.Usuarios.FirstOrDefault(u => u.Usuarioid == id);

                if (usuario == null)
                {
                    return NotFound($"Usuário com ID {id} não encontrado.");
                }

                _context.Usuarios.Remove(usuario);
                _context.SaveChanges();

                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao excluir usuário: {ex.Message}");
            }
        }
    }
}
