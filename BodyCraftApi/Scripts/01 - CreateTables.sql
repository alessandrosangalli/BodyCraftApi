CREATE TABLE `BodyCraftApiDb`.Foods (
	Id INT UNSIGNED auto_increment NOT NULL,
	Name varchar(100) NULL,
	ProteinPerGram float NULL,
	CarbPerGram float NULL,
	FatPerGram float NULL,
	CONSTRAINT Foods_PK PRIMARY KEY (Id)
)
ENGINE=InnoDB
DEFAULT CHARSET=utf8
COLLATE=utf8_general_ci;

CREATE TABLE BodyCraftApiDb.Meals (
	Id INT UNSIGNED auto_increment NOT NULL,
	FoodId INT UNSIGNED NOT NULL,
	`Time` TIME NOT NULL,
	QuantityInGrams FLOAT NOT NULL,
	CONSTRAINT Meals_PK PRIMARY KEY (Id),
	CONSTRAINT Meals_Foods_FK FOREIGN KEY (FoodId) REFERENCES BodyCraftApiDb.Foods(Id) ON DELETE RESTRICT ON UPDATE RESTRICT
)
ENGINE=InnoDB
DEFAULT CHARSET=utf8
COLLATE=utf8_general_ci;