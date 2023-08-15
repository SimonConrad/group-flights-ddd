using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


// alternatywy
// 1) article ma snapshot-model, który jest mapowany przez orm-a; na cele zapisu ma metode get-snapshot na cele utworzenia from-snapshot
// 2) article ma value object Tags zserializowany do json kolumny w article (HasConversion ma w definicji mappingu)
// może to byc kopia tagów, a tagi maja swoje niezależna tabele która nie musi byc spójna z wartościami w article
// lepiej chyba ownsmany - tagi nie moga istniec bez article, klucz obcy jednoczesnie klucz glowny w tagach, osobny readmodel z tagami aktualizowany gdy dodaje sie tagi do artykułu, tagi do stringi
// osobny model - readmodel z osobnym mapowaniem


public class Article
{
    public Guid ArticleId { get; private set; }
    public string Title { get; private set; }
    public string Content { get; private set; }

    private ICollection<Tag> _tags = new List<Tag>();
    public IEnumerable<Tag> Tags => _tags.ToList().AsReadOnly();

    private Article()
    {
    }
    
    private Article(Guid id)
    {
        ArticleId = id;
    }

    // metoda wytwórcza
    public static Article Create()
    {
        return new Article(Guid.NewGuid());
    }

    // scentralizowanie reguły pilnującej by liczba tagów w artykule nie przekraczała 10
    // enkapsulacja zmiana poprzez metodę nie przez setter
    // fajnie testowalne
    public void SetTags(List<Tag> tags)
    {
        if (_tags.Count() > 10)
        {
            // lepiej własny exception
            throw new InvalidOperationException("Cannot add more than 10 tags to an article.");
        }

        _tags.Clear();
        foreach (var tag in tags)
        {
            _tags.Add(tag);
        }
    }

    public void SetArticleDetails(string title, string content)
    {
        // todo można dodać walidacje i rzucić excpetionem jak złamana
        this.Title = title;
        this.Content = content;
    }
}

public class Tag
{
    public Guid TagId { get; set; }
    public string Name { get; set; }

    private ICollection<Article> _articles = new List<Article>();

    public IEnumerable<Article> Articles => _articles.ToList();
}



public class ArticleConfiguration : IEntityTypeConfiguration<Article>
{
    public void Configure(EntityTypeBuilder<Article> builder)
    {
        builder.ToTable("Articles");

        builder.HasKey(a => a.ArticleId);

        builder.Property(a => a.Title).IsRequired();
        builder.Property(a => a.Content).IsRequired();

        builder.Metadata.FindNavigation(nameof(Article.Tags))
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(a => a.Tags)
            .WithMany(t => t.Articles)
            .UsingEntity(j => j.ToTable("ArticleTags"));
    }
}


public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable("Tags");

        builder.HasKey(t => t.TagId);

        builder.Property(t => t.Name).IsRequired();

        builder.Metadata.FindNavigation(nameof(Tag.Articles))
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class ExampleDbContext : DbContext
{
    public DbSet<Article> Articles { get; set; }
    public DbSet<Tag> Tags { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ExampleDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

}

// todo interface in application layer
public class ArticleCommandsRepo
{
    private readonly ExampleDbContext _ctxt;

    public ArticleCommandsRepo(ExampleDbContext ctxt)
    {
        _ctxt = ctxt;
    }

    public async Task<Article> Get(Guid id)
    {
        return await _ctxt.Articles.Include(a => a.Tags).
            Where(x => x.ArticleId == id).FirstOrDefaultAsync();
    }
    
    public async Task Update(Article article)
    {
        _ctxt.Articles.Update(article); //sprawdzić czy nie trzeba ręcznie dodać tagsów
        await _ctxt.SaveChangesAsync();
    }
    
    public async Task Add(Article article)
    {
        await _ctxt.Articles.AddAsync(article); //sprawdzić czy nie trzeba ręcznie dodać tagsów
        await _ctxt.SaveChangesAsync();
    }
}

// todo interface and readmodel in application layer
public class ArticleQueriesRepo
{
    private readonly ExampleDbContext _ctxt;

    public ArticleQueriesRepo(ExampleDbContext ctxt)
    {
        _ctxt = ctxt;
    }

    public async Task<List<ArticleReadModel>> GetAll()
    {
        var articles = await _ctxt.Articles
            .Include(a => a.Tags)
            .Select(a => new ArticleReadModel
            {
                ArticleId = a.ArticleId,
                Title = a.Title,
                Content = a.Content,
                Tags = a.Tags
            })
            .ToListAsync();

        return articles;
    }
}

public class ArticleReadModel
{
    public Guid ArticleId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public IEnumerable<Tag> Tags { get; set; }
}