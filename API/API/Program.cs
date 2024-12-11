using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Models; 

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDataContext>();
var app = builder.Build();

app.MapGet("/", () => "prova");

//POST: /api/aluno/cadastrar
app.MapPost("/api/aluno/cadastrar", ([FromBody] Aluno aluno, [FromServices] AppDataContext db) =>
{
    if (db.Alunos.Any(a => a.Nome == aluno.Nome && a.Sobrenome == aluno.Sobrenome))
    {
        return Results.BadRequest("Já existe um aluno com este nome e sobrenome.");
    }

    db.Alunos.Add(aluno);
    db.SaveChanges();
    return Results.Created($"/api/aluno/{aluno.Id}", aluno);
});

//POST: /api/imc/cadastrar
app.MapPost("/api/imc/cadastrar", ([FromBody] IMC imc, [FromServices] AppDataContext db) =>
{
    var aluno = db.Alunos.Find(imc.AlunoId);
    if (aluno == null)
    {
        return Results.BadRequest("Aluno não encontrado.");
    }

    imc.ValorIMC = imc.Peso / (imc.Altura * imc.Altura);
    imc.Classificacao = imc.ValorIMC switch
    {
        < 18.5 => "Magreza",
        >= 18.5 and < 24.9 => "Normal",
        >= 25 and < 29.9 => "Sobrepeso",
        _ => "Obesidade"
    };
    imc.DataCadastro = DateTime.Now;

    db.IMCs.Add(imc);
    db.SaveChanges();
    return Results.Created($"/api/imc/{imc.Id}", imc);
});

//GET: /api/imc/listar
app.MapGet("/api/imc/listar", ([FromServices] AppDataContext db) =>
{
    if (db.IMCs.Any())
    {
        var imcs = db.IMCs
            .Select(imc => new
            {
                imc.Id,
                imc.Peso,
                imc.Altura,
                imc.ValorIMC,
                imc.Classificacao,
                imc.DataCadastro,
                AlunoNome = imc.Aluno.Nome,
                AlunoSobrenome = imc.Aluno.Sobrenome
            })
            .ToList();

        return Results.Ok(imcs);
    }
    return Results.NotFound();
});

//GET: /api/imc/listarporaluno/{alunoId}
app.MapGet("/api/imc/listarporaluno/{alunoId}", ([FromServices] AppDataContext db, int alunoId) =>
{
    var aluno = db.Alunos.Find(alunoId);
    if (aluno == null)
    {
        return Results.NotFound("Aluno não encontrado.");
    }

    var imcsDoAluno = db.IMCs
        .Where(imc => imc.AlunoId == alunoId)
        .Select(imc => new
        {
            imc.Id,
            imc.Peso,
            imc.Altura,
            imc.ValorIMC,
            imc.Classificacao,
            imc.DataCadastro
        })
        .ToList();

    if (imcsDoAluno.Any())
    {
        return Results.Ok(new
        {
            Aluno = $"{aluno.Nome} {aluno.Sobrenome}",
            IMCs = imcsDoAluno
        });
    }

    return Results.NotFound("Nenhum IMC encontrado para este aluno.");
});

//PUT: /api/imc/alterar/{id}
app.MapPut("/api/imc/alterar/{id}", ([FromRoute] int id, [FromBody] IMC imcAlterado, [FromServices] AppDataContext db) =>
{
    var imc = db.IMCs.Find(id);
    if (imc == null)
    {
        return Results.NotFound("IMC não encontrado.");
    }

    var aluno = db.Alunos.Find(imcAlterado.AlunoId);
    if (aluno == null)
    {
        return Results.NotFound("Aluno não encontrado.");
    }

    imc.AlunoId = imcAlterado.AlunoId;
    imc.Peso = imcAlterado.Peso;
    imc.Altura = imcAlterado.Altura;
    imc.ValorIMC = imc.Peso / (imc.Altura * imc.Altura);
    imc.Classificacao = imc.ValorIMC switch
    {
        < 18.5 => "Abaixo do peso",
        >= 18.5 and < 24.9 => "Peso normal",
        >= 25 and < 29.9 => "Sobrepeso",
        _ => "Obesidade"
    };

    db.IMCs.Update(imc);
    db.SaveChanges();
    return Results.Ok(imc);
});

app.Run();

