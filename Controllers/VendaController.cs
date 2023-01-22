using Microsoft.AspNetCore.Mvc;
using tech_test_payment_api.Context;
using tech_test_payment_api.Models;

namespace tech_test_payment_api.Controllers
{
    [Route("[controller]")]
    public class VendaController : Controller
    {
        private readonly ApiContext _logger;

        public VendaController(ApiContext logger)
        {
            _logger = logger;
        }
    
        [HttpPost("CadastrarVenda")]
        public IActionResult CadastrarVenda(int IdVendedor, List<int> IdProdutos){
            Vendedor vendedor = _logger.Vendedores.Find(IdVendedor);
            if (vendedor == null){
                return NotFound("Vendedor não encontrado");
            }
            else if(IdProdutos.Count == 0){
                return BadRequest("A lista precisa ter um item ou mais");
            }

            List<Produto> produtos = new List<Produto>();
            
            foreach (int Id in IdProdutos){
                Produto produto = _logger.Produtos.Find(Id);
                if (produto == null){
                    return NotFound("Produto não encontrado");
                }
                produtos.Add(produto);
            }

            Venda venda = new Venda(vendedor.IdVendedor, produtos);

            _logger.Vendas.Add(venda);
            _logger.SaveChanges();
            return Ok(venda);
        }

        [HttpGet("BuscarVenda{Id}")]
        public IActionResult BuscarVenda(int Id){
            var venda = _logger.Vendas.Find(Id);
            if (venda == null){
                return NotFound("Venda não encontrada");
            }
            return Ok(venda);
        }

        [HttpGet("ListarTodasAsVendas")]
        public IActionResult BuscarTodasAsVendas(){
            return Ok(_logger.Vendas.ToList());
        }

        [HttpPatch("AtualizarStatus")]
        public IActionResult AtualizarStatus(int Id, EnumStatusVenda status){
            string opcaoEnum = status.ToString();
            var venda = _logger.Vendas.Find(Id);
            if (venda == null){
                return NotFound("Venda não encontrada");
            }
            else{
                switch (venda.Status){
                    case "AguardandoPagamento":
                        if (!(opcaoEnum == "PagamentoAprovado" || opcaoEnum == "Cancelada"))
                        { return BadRequest("No status de AguardandoPagamento é possível apenas retornar ou 'PagamentoAprovado' ou 'Cancelada'"); }
                        break;
                    case "PagamentoAprovado":
                        if (!(opcaoEnum == "EnviadoParaTransportadora" || opcaoEnum == "Cancelada"))
                        { return BadRequest("No status de PagamentoAprovado é possível apenas retornar ou 'EnviadoParaTransportadora' ou 'Cancelada'");}
                        break;
                    case "EnviadoParaTransportadora":
                        if (!(opcaoEnum == "Entregue" || opcaoEnum == "Cancelada"))
                        { return BadRequest("No status de EnviadoParaTransportadora é possível apenas retornar ou 'Entregue' ou 'Cancelada'");}
                        break;
                }
            }
            venda.Status = opcaoEnum;
            _logger.Vendas.Update(venda);
            _logger.SaveChanges();
            return Ok(venda);
        }

        [HttpDelete("ExcluirVenda{Id}")]
        public IActionResult ExcluirVenda(int Id){
            var venda = _logger.Vendas.Find(Id);
            if (venda == null){
                return BadRequest("Venda Não Encontrada");
            }

            _logger.Vendas.Remove(venda);
            _logger.SaveChanges();
            return NoContent();
        }
    }
}