CREATE SCHEMA `sensors` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_bin ;;

CREATE TABLE `sensor_types` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(255) NOT NULL,
  `prefix` VARCHAR(10) NOT NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_bin;

CREATE TABLE `sensors` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `type` INT NOT NULL,
  `name` VARCHAR(20) NOT NULL,
  `description` VARCHAR(255) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `UQ_sensors` (`name`),
  KEY `FK__sensors__sensor_types_idx` (`type`),
  CONSTRAINT `FK__sensors__sensor_types` FOREIGN KEY (`type`) REFERENCES `sensor_types` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_bin;

CREATE TABLE `sensors_values` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `sensor_id` INT NOT NULL,
  `scanned_at` INT NOT NULL,
  `added_at` INT NOT NULL,
  `value` FLOAT NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `FK__sensors_values__sensors_idx` (`sensor_id` ASC),
  CONSTRAINT `FK__sensors_values__sensors`
    FOREIGN KEY (`sensor_id`)
    REFERENCES `sensors` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_bin;


INSERT INTO `sensor_types` (`id`,`name`, `prefix`) VALUES (1, 'Temperature Sensor DS18B20', '28-');
INSERT INTO `sensor_types` (`id`, `name`, `prefix`) VALUES (2, 'Humiture Sensot DHT11', 'dht11-');
INSERT INTO `sensor_types` (`id`, `name`, `prefix`) VALUES (3, 'Barometer', 'bmp180-');

INSERT INTO `sensors` (`id`, `type`, `name`, `description`) VALUES ('1', '1', '28-0121122a9970', 'Sensor1');
INSERT INTO `sensors` (`id`, `type`, `name`, `description`) VALUES ('2', '1', '28-01211268f329', 'Sensor2');
INSERT INTO `sensors` (`id`, `type`, `name`, `description`) VALUES ('3', '1', '28-012112412e4c', 'Sensor3');
INSERT INTO `sensors` (`id`, `type`, `name`, `description`) VALUES ('4', '1', '28-01211228258a', 'Sensor4');
INSERT INTO `sensors` (`id`, `type`, `name`, `description`) VALUES ('5', '1', '28-012112665040', 'Sensor5');
INSERT INTO `sensors` (`id`, `type`, `name`, `description`) VALUES ('6', '2', 'dht11-t', 'Humiture - temperature');
INSERT INTO `sensors` (`id`, `type`, `name`, `description`) VALUES ('7', '2', 'dht11-h', 'Humiture - humidity');
INSERT INTO `sensors` (`id`, `type`, `name`, `description`) VALUES ('8', '3', 'bmp180-t', 'Barometer-Temperature');
INSERT INTO `sensors` (`id`, `type`, `name`, `description`) VALUES ('9', '3', 'bmp180-p', 'Barometer-Pressure');
INSERT INTO `sensors` (`id`, `type`, `name`, `description`) VALUES ('10', '3', 'bmp180-a', 'Barometer-Altitude');


