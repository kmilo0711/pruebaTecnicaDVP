require 'mongo'

module Database
  def self.client
    @client ||= Mongo::Client.new('mongodb://admin:admin123@localhost:27017/auditoria_db?authSource=admin')
  end

  def self.eventos_collection
    client[:eventos]
  end
end
