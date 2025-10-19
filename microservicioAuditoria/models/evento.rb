require_relative '../config/database'

class Evento
  def self.create(data)
    normalized_data = normalizar_datos(data)

    required_fields = ['servicio', 'entidad', 'entidadId', 'accion']
    missing_fields = required_fields.select do |field|
      value = normalized_data[field]
      value.nil? || value.to_s.strip.empty?
    end

    unless missing_fields.empty?
      raise ArgumentError, "Campos requeridos faltantes: #{missing_fields.join(', ')}"
    end

    evento_data = normalized_data.merge({
      'timestamp' => Time.now.utc,
      'fechaCreacion' => Time.now.utc
    })

    result = Database.eventos_collection.insert_one(evento_data)
    evento_data.merge('_id' => result.inserted_id)
  end

  def self.find_by_entidad_id(entidad_id)
    Database.eventos_collection.find({ 'entidadId' => entidad_id.to_s })
                               .sort({ 'timestamp' => -1 })
                               .to_a
  end

  def self.all
    Database.eventos_collection.find({})
                               .sort({ 'timestamp' => -1 })
                               .to_a
  end

  def self.normalizar_datos(data)
    mapping = {
      'servicio' => 'servicio',
      'entidad' => 'entidad',
      'entidadid' => 'entidadId',
      'entidad_id' => 'entidadId',
      'accion' => 'accion',
      'detalles' => 'detalles',
      'timestamp' => 'timestamp',
      'fecha' => 'fecha',
      'fechacreacion' => 'fechaCreacion'
    }

    normalized = {}

    data.each do |key, value|
      normalized_key = mapping[key.to_s.strip.downcase] || key.to_s
      normalized[normalized_key] = value
    end

    if normalized['entidadId']
      normalized['entidadId'] = normalized['entidadId'].to_s
    end

    normalized
  end
end
