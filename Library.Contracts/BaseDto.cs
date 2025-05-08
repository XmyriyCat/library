using Mapster;

namespace Library.Contracts;

public abstract class BaseDto<TDto, TEntity> : IRegister
    where TDto : class, new()
    where TEntity : class, new()
{
    private TypeAdapterConfig _config { get; set; }

    public virtual void AddCustomMappings()
    {
    }

    protected TypeAdapterSetter<TDto, TEntity> SetCustomMappings()
        => _config.ForType<TDto, TEntity>();

    protected TypeAdapterSetter<TEntity, TDto> SetCustomMappingsInverse()
        => _config.ForType<TEntity, TDto>();

    public void Register(TypeAdapterConfig config)
    {
        _config = config;
        AddCustomMappings();
    }

    public TEntity ToEntity()
    {
        return this.Adapt<TEntity>();
    }

    public TEntity ToEntity(TEntity entity)
    {
        return (this as TDto).Adapt(entity);
    }

    public static TDto FromEntity(TEntity entity)
    {
        return entity.Adapt<TDto>();
    }
}