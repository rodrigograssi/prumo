using Microsoft.EntityFrameworkCore;
using PesquisaX.Shared.Modelos;
using PesquisaX.Shared;

namespace PesquisaX.Infra.Contexto;

public class PesquisaXContexto : DbContext
{
    public PesquisaXContexto(DbContextOptions<PesquisaXContexto> options) : base(options)
    {
    }

    public DbSet<Pesquisa> Pesquisas { get; set; } = null!;
    public DbSet<GrupoQuestao> GruposQuestoes { get; set; } = null!;
    public DbSet<Questao> Questoes { get; set; } = null!;
    public DbSet<ResultadoPessoa> ResultadosPessoas { get; set; } = null!;
    public DbSet<Resposta> Respostas { get; set; } = null!;
    public DbSet<Usuario> Usuarios { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuração da entidade Pesquisa
        modelBuilder.Entity<Pesquisa>(entity =>
        {
            entity.ToTable("pesquisas");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)
                .HasColumnName("id");
                
            entity.Property(e => e.Nome)
                .HasColumnName("nome")
                .HasMaxLength(200)
                .IsRequired();
                
            entity.Property(e => e.DataCriacao)
                .HasColumnName("data_criacao")
                .IsRequired();

            // Relacionamento um-para-muitos com GrupoQuestao
            entity.HasMany(p => p.GruposQuestoes)
                .WithOne()
                .HasForeignKey("pesquisa_id")
                .OnDelete(DeleteBehavior.Cascade);

            // Relacionamento um-para-muitos com ResultadoPessoa
            entity.HasMany(p => p.Resultados)
                .WithOne()
                .HasForeignKey("pesquisa_id")
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuração da entidade GrupoQuestao
        modelBuilder.Entity<GrupoQuestao>(entity =>
        {
            entity.ToTable("grupos_questoes");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)
                .HasColumnName("id");
                
            entity.Property(e => e.Nome)
                .HasColumnName("nome")
                .HasMaxLength(200)
                .IsRequired();
                
            entity.Property(e => e.Categoria)
                .HasColumnName("categoria")
                .HasMaxLength(100)
                .IsRequired();
                
            entity.Property(e => e.Cor)
                .HasColumnName("cor")
                .HasMaxLength(7)
                .HasDefaultValue("#FF6B6B");

            entity.Property("pesquisa_id")
                .HasColumnName("pesquisa_id");

            // Relacionamento um-para-muitos com Questao
            entity.HasMany(g => g.Questoes)
                .WithOne()
                .HasForeignKey("grupo_questao_id")
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuração da entidade Questao
        modelBuilder.Entity<Questao>(entity =>
        {
            entity.ToTable("questoes");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)
                .HasColumnName("id");
                
            entity.Property(e => e.Texto)
                .HasColumnName("texto")
                .HasMaxLength(500)
                .IsRequired();
                
            entity.Property(e => e.EInvertida)
                .HasColumnName("e_invertida")
                .HasDefaultValue(false);

            entity.Property("grupo_questao_id")
                .HasColumnName("grupo_questao_id");
        });

        // Configuração da entidade ResultadoPessoa
        modelBuilder.Entity<ResultadoPessoa>(entity =>
        {
            entity.ToTable("resultados_pessoas");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)
                .HasColumnName("id");
                
            entity.Property(e => e.NomePessoa)
                .HasColumnName("nome_pessoa")
                .HasMaxLength(200)
                .IsRequired();
                
            entity.Property(e => e.Email)
                .HasColumnName("email")
                .HasMaxLength(300);
                
            entity.Property(e => e.DataAvaliacao)
                .HasColumnName("data_avaliacao")
                .IsRequired();

            entity.Property("pesquisa_id")
                .HasColumnName("pesquisa_id");

            // Relacionamento um-para-muitos com Resposta
            entity.HasMany(r => r.Respostas)
                .WithOne()
                .HasForeignKey("resultado_pessoa_id")
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuração da entidade Resposta
        modelBuilder.Entity<Resposta>(entity =>
        {
            entity.ToTable("respostas");
            
            // Chave composta: QuestaoId + ResultadoPessoaId
            entity.HasKey("QuestaoId", "resultado_pessoa_id");
            
            entity.Property(e => e.QuestaoId)
                .HasColumnName("questao_id");
                
            entity.Property(e => e.ValorBruto)
                .HasColumnName("valor_bruto")
                .IsRequired();
                
            entity.Property(e => e.ValorProcessado)
                .HasColumnName("valor_processado")
                .IsRequired();

            // Relacionamento com Questao
            entity.HasOne<Questao>()
                .WithMany()
                .HasForeignKey(r => r.QuestaoId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuração da entidade Usuario
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("usuarios");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)
                .HasColumnName("id");
                
            entity.Property(e => e.Nome)
                .HasColumnName("nome")
                .HasMaxLength(100)
                .IsRequired();
                
            entity.Property(e => e.Email)
                .HasColumnName("email")
                .HasMaxLength(255)
                .IsRequired();
                
            entity.Property(e => e.Senha)
                .HasColumnName("senha")
                .IsRequired();
                
            entity.Property(e => e.DataCriacao)
                .HasColumnName("data_criacao")
                .IsRequired();

            // Índice único para email
            entity.HasIndex(e => e.Email)
                .IsUnique()
                .HasDatabaseName("ix_usuarios_email");
        });
    }
}