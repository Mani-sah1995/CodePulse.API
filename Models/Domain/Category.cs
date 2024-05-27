namespace CodePulse.API.Models.Domain
{
    
    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string UrlHandle { get; set; }

        //One Category can have multiple blog post
        public ICollection<BlogPost> BlogPost { get; set; }
    }
}
