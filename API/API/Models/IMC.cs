using API.Models;
public class IMC
{
    public int Id { get; set; }
    public double Altura { get; set; }
    public double Peso { get; set; }
    public double ValorIMC { get; set; }
    public string? Classificacao { get; set; }
    public DateTime DataCadastro { get; set; }
    public int AlunoId { get; set; }
    public Aluno? Aluno { get; set; }

}
