USE Gringotts

--01. Records’ Count
SELECT COUNT(Id) AS Count 
  FROM WizzardDeposits

--02. Longest Magic Wand
SELECT MAX(MagicWandSize) AS LongestMagicWand 
  FROM WizzardDeposits

--03. Longest Magic Wand per Deposit Groups
  SELECT DepositGroup, MAX(MagicWandSize) AS LongestMagicWand 
    FROM WizzardDeposits
GROUP BY DepositGroup

--04. Smallest Deposit Group per Magic Wand Size
SELECT TOP(2) DepositGroup 
      FROM WizzardDeposits
  GROUP BY DepositGroup
  ORDER BY AVG(MagicWandSize)

--05. Deposits Sum
  SELECT DepositGroup, 
		 SUM(DepositAmount) AS TotalSum
    FROM WizzardDeposits
GROUP BY DepositGroup

--06. Deposits Sum for Ollivander Family
  SELECT DepositGroup,
	     SUM(DepositAmount) AS TotalSum
    FROM WizzardDeposits
   WHERE MagicWandCreator = 'Ollivander family'
GROUP BY DepositGroup

--07. Deposits Filter
  SELECT DepositGroup,
		 SUM(DepositAmount) AS TotalSum
    FROM WizzardDeposits
   WHERE MagicWandCreator = 'Ollivander family'
GROUP BY DepositGroup
  HAVING SUM(DepositAmount) < 150000
ORDER BY TotalSum DESC

--08. Deposit Charge
  SELECT DepositGroup, MagicWandCreator, MIN(DepositCharge) AS MinDepositCharge
    FROM WizzardDeposits
GROUP BY DepositGroup, MagicWandCreator
ORDER BY MagicWandCreator, DepositGroup 

--09. Age Groups
  SELECT 
  		CASE
  		     WHEN Age <= 10 THEN '[0-10]'
  		     WHEN Age <= 20 THEN '[11-20]'
  		     WHEN Age <= 30 THEN '[21-30]'
  		     WHEN Age <= 40 THEN '[31-40]'
  		     WHEN Age <= 50 THEN '[41-50]'
  		     WHEN Age <= 60 THEN '[51-60]'
  		     ELSE '[61+]'
  		END AS [AgeGroup], 
  		COUNT(Id) AS WizardCount
    FROM WizzardDeposits
GROUP BY (CASE
		       WHEN Age <= 10 THEN '[0-10]'
		       WHEN Age <= 20 THEN '[11-20]'
		       WHEN Age <= 30 THEN '[21-30]'
		       WHEN Age <= 40 THEN '[31-40]'
		       WHEN Age <= 50 THEN '[41-50]'
		       WHEN Age <= 60 THEN '[51-60]'
		       ELSE '[61+]'
		   END)

--10. First Letter
  SELECT LEFT(FirstName, 1) AS FirstLetter
    FROM WizzardDeposits
   WHERE DepositGroup = 'Troll Chest'
GROUP BY LEFT(FirstName, 1)
ORDER BY LEFT(FirstName, 1)

--11. Average Interest
  SELECT DepositGroup, IsDepositExpired,
		 AVG(DepositInterest) AS AverageInterest
    FROM WizzardDeposits
   WHERE DepositStartDate > '1985-01-01'
GROUP BY DepositGroup, IsDepositExpired
ORDER BY DepositGroup DESC, IsDepositExpired

--12. Rich Wizard, Poor Wizard
SELECT
	   SUM(t.[Difference]) AS SumDifference
  FROM
       (SELECT 
       	       --FirstName AS [Host Wizard], 
       	       --DepositAmount AS [Host Wizard Deposit], 
       	       --LEAD(FirstName, 1, NULL) OVER (ORDER BY Id) AS [Guest Wizzard], 
       	       --LEAD(DepositAmount, 1, NULL) OVER (ORDER BY Id) AS [Guest Wizard Deposit],
       	       DepositAmount -  LEAD(DepositAmount, 1, NULL) OVER (ORDER BY Id) AS [Difference]	   
          FROM WizzardDeposits) AS t

SELECT * FROM WizzardDeposits