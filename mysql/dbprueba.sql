CREATE TABLE categoria (
  id int(11) primary key NOT NULL AUTO_INCREMENT,
  nombre varchar(45) NOT NULL
);
CREATE TABLE articulo (
  id int(11) primary key NOT NULL AUTO_INCREMENT,
  nombre varchar(45) NOT NULL,
  categoria int(11) references categoria (id),
  precio decimal(10,2)
)


