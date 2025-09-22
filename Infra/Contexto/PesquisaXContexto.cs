using Microsoft.EntityFrameworkCore;
using PesquisaX.Core;
using PesquisaX.Core.Modelos;

namespace PesquisaX.Infra.Contexto;

public class PesquisaXContexto : DbContext
{
    public PesquisaXContexto(DbContextOptions<PesquisaXContexto> options) : base(options)
    {
    }

    public DbSet<Pesquisa> Pesquisas { get; set; } = null!;
    public DbSet<GrupoQuestao> GruposQuestoes { get; set; } = null!;
    public DbSet<Questao> Questoes { get; set; } = null!;
    public DbSet<RespostaPessoa> ResultadosPessoas { get; set; } = null!;
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
                .HasColumnName("id")
                .ValueGeneratedOnAdd();
                
            entity.Property(e => e.PublicId)
                .HasColumnName("public_id")
                .IsRequired();
                
            entity.Property(e => e.Nome)
                .HasColumnName("nome")
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(e => e.NomeDoResultadoDesejado)
                .HasColumnName("nome_do_resultado_desejado")
                .HasMaxLength(200)
                .HasDefaultValue("Resultado Desejado");

            entity.Property(e => e.DataCriacao)
                .HasColumnName("data_criacao")
                .IsRequired();

            // Relacionamento um-para-muitos com GrupoQuestao
            entity.HasMany(p => p.GruposQuestoes)
                .WithOne()
                .HasForeignKey( g=> g.PesquisaId )
                .OnDelete(DeleteBehavior.Cascade);

            // Relacionamento um-para-muitos com ResultadoPessoa
            entity.HasMany(p => p.RespostasPessoa)
                .WithOne()
                .HasForeignKey(r => r.PesquisaId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuração da entidade GrupoQuestao
        modelBuilder.Entity<GrupoQuestao>(entity =>
        {
            entity.ToTable("grupos_questoes");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();
                
            entity.Property(e => e.PublicId)
                .HasColumnName("public_id")
                .IsRequired();
                
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

            entity.Property(g => g.PesquisaId)
                .HasColumnName("pesquisa_id");

            // Relacionamento um-para-muitos com Questao
            entity.HasMany(g => g.Questoes)
                .WithOne()
                .HasForeignKey(q => q.GrupoDeQuestaoId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuração da entidade Questao
        modelBuilder.Entity<Questao>(entity =>
        {
            entity.ToTable("questoes");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();
                
            entity.Property(e => e.PublicId)
                .HasColumnName("public_id")
                .IsRequired();
                
            entity.Property(e => e.Texto)
                .HasColumnName("texto")
                .HasMaxLength(500)
                .IsRequired();
                
            entity.Property(e => e.EInvertida)
                .HasColumnName("e_invertida")
                .HasDefaultValue(false);

            entity.Property(e => e.GrupoDeQuestaoId)
                .HasColumnName("grupo_questao_id");
        });

        // Configuração da entidade ResultadoPessoa
        modelBuilder.Entity<RespostaPessoa>(entity =>
        {
            entity.ToTable("resultados_pessoas");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();
                
            entity.Property(e => e.PublicId)
                .HasColumnName("public_id")
                .IsRequired();
                
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

            entity.Property(e => e.PesquisaId)
                .HasColumnName("pesquisa_id");

        });

        // Configuração da entidade Resposta
        modelBuilder.Entity<Resposta>(entity =>
        {
            entity.ToTable("respostas");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();
                
            entity.Property(e => e.PublicId)
                .HasColumnName("public_id")
                .IsRequired();
            
            entity.Property(e => e.QuestaoId)
                .HasColumnName("questao_id");
                
            entity.Property(e => e.ResultadoPessoaId)
                .HasColumnName("resultado_pessoa_id");
                
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
            
            // Relacionamento com ResultadoPessoa
            entity.HasOne<RespostaPessoa>()
                .WithMany(rp => rp.Respostas)
                .HasForeignKey(r => r.ResultadoPessoaId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuração da entidade Usuario
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("usuarios");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();
                
            entity.Property(e => e.PublicId)
                .HasColumnName("public_id")
                .IsRequired();
                
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

            entity.OwnsOne(e => e.Sistema, sist =>
            {
                sist.Property(x => x.DataUltimoAcesso)
                    .HasColumnName("data_ultimo_acesso");
                sist.Property(x => x.Token)
                    .HasColumnName("token");
            });

        });
    }
}