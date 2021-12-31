CREATE SCHEMA `sensors` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_bin ;;

CREATE TABLE `sensor_types` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(255) NOT NULL,
  `prefix` VARCHAR(5) NOT NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_bin;

CREATE TABLE `sensors` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `type` INT NOT NULL,
  `name` VARCHAR(255) NOT NULL,
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
