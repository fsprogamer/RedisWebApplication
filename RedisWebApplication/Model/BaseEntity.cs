namespace RedisWebApplication.Model
{
    public abstract class BaseEntity<K>
    {        
        public K Id { get; set; }    
    }   
}
