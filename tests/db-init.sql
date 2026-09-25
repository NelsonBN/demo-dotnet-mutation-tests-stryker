CREATE TABLE `demo`.`Product` (
  `Id` char(36) NOT NULL,
  `Name` varchar(50) NOT NULL,
  `Quantity` int(11) unsigned NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB ;


INSERT INTO `demo`.`Product`
    (`Id`, `Name`, `Quantity`)
VALUES
    ('412548eb-f19b-4049-be67-d7fcf4d19461', 'Motherboard', 23),
    ('8178df7a-da1e-4205-8e9c-eeecef01f3d7', 'Keyboard', 4),
    ('1c440211-66e0-4019-afea-0b0299ae599a', 'Mouse', 7),
    ('31e88e92-b534-4121-a201-8027d805f86c', 'Monitor', 15);
