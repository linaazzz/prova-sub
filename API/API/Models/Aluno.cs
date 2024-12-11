namespace API.Models;

public class Aluno{

    public int Id { get; set; }
    public string? Nome { get; set; }
    public string? Sobrenome { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.Now;
}