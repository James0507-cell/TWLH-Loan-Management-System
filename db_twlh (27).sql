-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Mar 14, 2026 at 03:53 PM
-- Server version: 10.4.32-MariaDB
-- PHP Version: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `db_twlh`
--

DELIMITER $$
--
-- Procedures
--
CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_execute_waterfall_payment` (IN `p_trans_id` INT, IN `p_client_id` INT, IN `p_total_amount` DECIMAL(12,2), IN `p_user_id` INT)   begin
declare v_remaining decimal(12,2) default p_total_amount;
declare v_inst_id int;
declare v_inst_due decimal(12,2);

payment_loop: while v_remaining > 0 do
set v_inst_id = null;

select installment_id, total_amount_to_pay
into v_inst_id, v_inst_due
from vw_total_amount_installment
where client_id = p_client_id
and installment_status in ('Active', 'Past Due')
order by installment_due_date asc
limit 1;

if v_inst_id is null then
leave payment_loop;
end if;

set @current_payment = if(v_remaining >= v_inst_due, v_inst_due, v_remaining);

insert into tbl_installment_payment (
installment_id,
transaction_id,
payment_amount,
recorded_by
)
values (
v_inst_id,
p_trans_id,
@current_payment,
p_user_id
);

set v_remaining = v_remaining - @current_payment;

end while payment_loop;
end$$

DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table `tbl_admin`
--

CREATE TABLE `tbl_admin` (
  `admin_id` int(11) NOT NULL,
  `employee_id` int(11) NOT NULL,
  `is_active` tinyint(1) NOT NULL DEFAULT 1,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `tbl_admin`
--

INSERT INTO `tbl_admin` (`admin_id`, `employee_id`, `is_active`, `created_at`) VALUES
(1, 1, 1, '2026-02-11 06:55:18'),
(2, 7, 0, '2026-03-04 15:02:30'),
(3, 11, 1, '2026-03-14 05:13:00'),
(4, 12, 0, '2026-03-14 05:31:48');

-- --------------------------------------------------------

--
-- Table structure for table `tbl_business`
--

CREATE TABLE `tbl_business` (
  `business_id` int(11) NOT NULL,
  `client_id` int(11) NOT NULL,
  `business_name` varchar(50) NOT NULL,
  `business_address` text NOT NULL,
  `business_registration_id` varchar(50) NOT NULL,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `tbl_business`
--

INSERT INTO `tbl_business` (`business_id`, `client_id`, `business_name`, `business_address`, `business_registration_id`, `created_at`) VALUES
(1, 1, 'Dela Cruz Sari-Sari Store', 'Davao City', 'DTI-2026-0001', '2026-02-11 06:52:08'),
(2, 2, 'Garcia Mini Mart', 'Tagum City', 'DTI-2026-0002', '2026-02-11 06:52:08'),
(3, 3, 'Torres Hardware Supply', 'Panabo City', 'DTI-2026-0003', '2026-02-11 06:52:08'),
(4, 4, 'Flores Online Shop', 'Digos City', 'DTI-2026-0004', '2026-02-11 06:52:08'),
(5, 5, 'Villanueva Eatery', 'Samal Island', 'DTI-2026-0005', '2026-02-11 06:52:08'),
(6, 1, 'SecretStore 123', 'Davao City', 'DTI-2026-2007', '2026-02-28 07:47:29'),
(8, 6, 'AlleNhic Store', 'Bugac Ma-a', '234653', '2026-03-05 14:26:37'),
(9, 8, 'Allenhic Store', 'Maa', 'str:0923', '2026-03-13 17:52:40');

-- --------------------------------------------------------

--
-- Table structure for table `tbl_client`
--

CREATE TABLE `tbl_client` (
  `client_id` int(11) NOT NULL,
  `first_name` varchar(50) NOT NULL,
  `middle_name` varchar(50) NOT NULL,
  `last_name` varchar(50) NOT NULL,
  `gender` enum('Male','Female','Others') NOT NULL,
  `date_of_birth` date NOT NULL,
  `contact_number` varchar(12) NOT NULL,
  `current_residence` text NOT NULL,
  `messenger_name` varchar(100) NOT NULL,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp(),
  `updated_by` int(11) DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL ON UPDATE current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `tbl_client`
--

INSERT INTO `tbl_client` (`client_id`, `first_name`, `middle_name`, `last_name`, `gender`, `date_of_birth`, `contact_number`, `current_residence`, `messenger_name`, `created_at`, `updated_by`, `updated_at`) VALUES
(1, 'Juan', 'Santos', 'Dela Cruz', 'Others', '1998-05-12', '09171234567', 'Davao City', 'juan.dc', '2026-02-11 06:50:24', NULL, '2026-03-05 16:13:53'),
(2, 'Maria', 'Lopez', 'Garcia', 'Others', '2000-08-25', '09182345678', 'Tagum City', 'maria.g', '2026-02-11 06:50:24', NULL, '2026-03-05 16:13:37'),
(3, 'Kevin', 'Reyes', 'Torres', 'Male', '1995-11-03', '09193456789', 'Panabo City', 'kevin.t', '2026-02-11 06:50:24', NULL, '2026-03-05 16:14:41'),
(4, 'Angela', 'Mendoza', 'Flores', 'Female', '1999-02-14', '09204567891', 'Digos City', 'angela.f', '2026-02-11 06:50:24', NULL, NULL),
(5, 'Chris', 'Ramos', 'Villanueva', 'Others', '2001-07-19', '09215678912', 'Samal Island', 'chris.v', '2026-02-11 06:50:24', NULL, NULL),
(6, 'James', 'Montojo', 'Santiago', 'Male', '2006-05-07', '09543532535', 'Bugac Maa', 'James Santiago', '2026-03-05 14:26:06', NULL, NULL),
(7, 'Lester', 'Santiago', 'Arigo', 'Male', '2016-02-06', '344322', 'Maa', 'Lester', '2026-03-12 06:53:08', NULL, NULL),
(8, 'Marco', 'Montojo', 'Santiago', 'Male', '1976-08-04', '0963545322', 'Maa', 'John Marco', '2026-03-13 17:52:10', NULL, NULL),
(9, 'Marco', 'Monjo', 'Santiago', 'Others', '1996-03-07', '0965456543', 'Maa', 'John Marco', '2026-03-14 05:10:18', NULL, NULL),
(10, 'Julian', 'Arigo', 'Tandug', 'Others', '2010-03-04', '0956346754', 'Mintal', 'Julian', '2026-03-14 05:29:40', NULL, NULL);

-- --------------------------------------------------------

--
-- Table structure for table `tbl_collection_assignment`
--

CREATE TABLE `tbl_collection_assignment` (
  `assignment_id` int(11) NOT NULL,
  `past_due_id` int(11) NOT NULL,
  `assigned_to` int(11) NOT NULL,
  `assignment_status` enum('In Progress','Completed','Canceled') NOT NULL,
  `created_by` int(11) NOT NULL,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp(),
  `updated_by` int(11) DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL ON UPDATE current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `tbl_collection_assignment`
--

INSERT INTO `tbl_collection_assignment` (`assignment_id`, `past_due_id`, `assigned_to`, `assignment_status`, `created_by`, `created_at`, `updated_by`, `updated_at`) VALUES
(9, 6, 3, 'In Progress', 2, '2026-02-11 14:27:18', 1, NULL),
(10, 7, 5, 'In Progress', 2, '2026-02-11 14:27:18', 1, NULL),
(11, 8, 3, 'In Progress', 2, '2026-02-11 14:27:18', 1, NULL),
(12, 11, 5, 'Completed', 2, '2026-02-27 16:16:12', 1, '2026-03-05 16:12:03'),
(13, 24, 3, 'Canceled', 1, '2026-03-05 14:56:55', 1, NULL),
(14, 30, 3, 'In Progress', 1, '2026-03-12 06:31:29', NULL, NULL),
(15, 31, 3, 'Completed', 1, '2026-03-13 17:48:36', 1, '2026-03-13 17:50:25'),
(16, 32, 3, 'In Progress', 1, '2026-03-14 04:59:54', NULL, NULL),
(17, 33, 3, 'Completed', 1, '2026-03-14 05:07:12', 1, '2026-03-14 05:08:34'),
(18, 35, 3, 'In Progress', 1, '2026-03-14 05:20:08', NULL, NULL),
(19, 36, 3, 'Completed', 1, '2026-03-14 05:26:00', 1, '2026-03-14 05:27:51');

--
-- Triggers `tbl_collection_assignment`
--
DELIMITER $$
CREATE TRIGGER `trg_before_assignment_update` BEFORE UPDATE ON `tbl_collection_assignment` FOR EACH ROW begin
if not exists (
select 1 from tbl_collector where employee_id = new.assigned_to and is_active = 1
) then
signal sqlstate '45000'
set message_text = 'This emplloyee is not an active collector';
end if;
if not exists (
select 1 from tbl_admin where employee_id = new.updated_by and is_active = 1
union
select 1 from tbl_staff where employee_id = new.updated_by and is_active = 1
) then
signal sqlstate '45001'
set message_text = 'You are not authorize for updating colleciton assingment records.';
end if;
end
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `trg_before_collection_insert` BEFORE INSERT ON `tbl_collection_assignment` FOR EACH ROW begin
if not exists (
select 1 from tbl_collector
where employee_id = new.assigned_to
and is_active = 1
) then
signal sqlstate '45000'
set message_text = 'The assigned employee is not an active collector.';
end if;
if exists (select 1 from tbl_collector
where employee_id = new.created_by
and is_active = 1
) then
signal sqlstate '45001'
set message_text = 'You are not authorize to create a new collection assignment.';
end if;
end
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table `tbl_collector`
--

CREATE TABLE `tbl_collector` (
  `collector_id` int(11) NOT NULL,
  `employee_id` int(11) NOT NULL,
  `is_active` tinyint(1) NOT NULL DEFAULT 1,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `tbl_collector`
--

INSERT INTO `tbl_collector` (`collector_id`, `employee_id`, `is_active`, `created_at`) VALUES
(1, 3, 1, '2026-02-11 06:55:35'),
(2, 5, 1, '2026-02-11 06:55:35');

-- --------------------------------------------------------

--
-- Table structure for table `tbl_employee`
--

CREATE TABLE `tbl_employee` (
  `employee_id` int(11) NOT NULL,
  `first_name` varchar(50) NOT NULL,
  `middle_name` varchar(50) NOT NULL,
  `last_name` varchar(50) NOT NULL,
  `gender` enum('Male','Female','Others') NOT NULL,
  `date_of_birth` date NOT NULL,
  `role` enum('Admin','Staff','Loan Collector') NOT NULL,
  `contact_number` varchar(12) NOT NULL,
  `email` varchar(50) NOT NULL,
  `is_active` tinyint(1) NOT NULL DEFAULT 1,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp(),
  `updated_by` int(11) DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL ON UPDATE current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `tbl_employee`
--

INSERT INTO `tbl_employee` (`employee_id`, `first_name`, `middle_name`, `last_name`, `gender`, `date_of_birth`, `role`, `contact_number`, `email`, `is_active`, `created_at`, `updated_by`, `updated_at`) VALUES
(1, 'Carlos', 'Mendez', 'Rivera', 'Male', '1990-04-15', 'Admin', '09170000001', 'carlos.rivera@gmail.com', 1, '2026-02-11 06:52:53', 1, '2026-03-07 03:19:54'),
(2, 'Liza', 'Fernandez', 'Cruz', 'Female', '1993-09-21', 'Staff', '09170000002', 'liza.cruz@gmail.com', 1, '2026-02-11 06:52:53', NULL, NULL),
(3, 'Mark', 'Santos', 'Lopez', 'Male', '1988-12-05', 'Loan Collector', '09170000003', 'mark.lopez@gmail.com', 1, '2026-02-11 06:52:53', NULL, NULL),
(4, 'Jenny', 'Castillo', 'Reyes', 'Female', '1995-06-18', 'Staff', '09170000004', 'jenny.reyes@gmail.com', 1, '2026-02-11 06:52:53', NULL, NULL),
(5, 'Adrian', 'Torres', 'Gomez', 'Others', '1992-01-30', 'Loan Collector', '09170000005', 'adrian.gomez@gmail.com', 1, '2026-02-11 06:52:53', NULL, NULL),
(7, 'James', 'Montojo', 'Santiago', 'Female', '2006-05-07', 'Staff', '0934264532', 'jamessantiago@gmail.com', 1, '2026-03-04 15:02:30', NULL, '2026-03-05 16:14:50'),
(8, 'Marco', 'Montojo', 'Santiago', 'Male', '1995-03-15', 'Staff', '0965444534', 'Marco@gmail.com', 0, '2026-03-13 17:53:53', 1, '2026-03-13 17:54:20'),
(11, 'A', 'A', 'A', 'Female', '2026-03-04', 'Admin', '123643533', 'A@gmail.com', 1, '2026-03-14 05:13:00', NULL, NULL),
(12, 'Julian', 'Arigo', 'Tandug', 'Male', '2010-03-04', 'Admin', '0975675367', 'Tandug@gmail.com', 0, '2026-03-14 05:31:33', 1, '2026-03-14 05:31:52');

-- --------------------------------------------------------

--
-- Table structure for table `tbl_employee_credential`
--

CREATE TABLE `tbl_employee_credential` (
  `credential_id` int(11) NOT NULL,
  `employee_id` int(11) NOT NULL,
  `username` varchar(50) NOT NULL,
  `password_hash` varchar(255) NOT NULL,
  `is_active` tinyint(1) NOT NULL DEFAULT 1,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `tbl_employee_credential`
--

INSERT INTO `tbl_employee_credential` (`credential_id`, `employee_id`, `username`, `password_hash`, `is_active`, `created_at`) VALUES
(1, 1, 'admin.carlos', 'admin123', 1, '2026-02-11 06:53:55'),
(2, 2, 'staff.liza', 'staff123', 1, '2026-02-11 06:53:55'),
(3, 3, 'collector.mark', 'collector123', 1, '2026-02-11 06:53:55'),
(4, 4, 'staff.jenny', '$2y$10$hashedpasswordjenny', 1, '2026-02-11 06:53:55'),
(5, 5, 'collector.adrian', '$2y$10$hashedpasswordadrian', 1, '2026-02-11 06:53:55'),
(6, 7, 'jcSantiago', 'admin123', 1, '2026-03-04 15:02:30'),
(7, 8, 'marco123', 'marco123', 0, '2026-03-13 17:53:53'),
(8, 11, 'a', 'a', 1, '2026-03-14 05:13:00'),
(9, 12, 'tandug123', 'tandug123', 0, '2026-03-14 05:31:33');

-- --------------------------------------------------------

--
-- Table structure for table `tbl_follow_up`
--

CREATE TABLE `tbl_follow_up` (
  `follow_up_id` int(11) NOT NULL,
  `past_due_id` int(11) NOT NULL,
  `follow_up_date` date NOT NULL,
  `follow_up_type` enum('Call','Message','Email','Meet Up') NOT NULL,
  `notes` text DEFAULT NULL,
  `recorded_by` int(11) NOT NULL,
  `updated_by` int(11) DEFAULT NULL,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp(),
  `updated_at` timestamp NULL DEFAULT NULL ON UPDATE current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `tbl_follow_up`
--

INSERT INTO `tbl_follow_up` (`follow_up_id`, `past_due_id`, `follow_up_date`, `follow_up_type`, `notes`, `recorded_by`, `updated_by`, `created_at`, `updated_at`) VALUES
(6, 6, '2026-02-11', 'Call', 'Called the client; they confirmed they will pay the full balance by Feb 18.', 1, 1, '2026-02-11 14:23:39', '2026-03-05 16:12:29'),
(7, 7, '2026-02-11', 'Message', 'Sent a reminder via Messenger. Client replied saying they will pay after their payday on the 20th.', 2, NULL, '2026-02-11 14:23:39', NULL),
(8, 8, '2026-02-11', 'Call', 'Spoke with the client. They had issues with the app but promised to pay at the office on Feb 25.', 2, NULL, '2026-02-11 14:23:39', NULL),
(9, 24, '2026-03-05', 'Call', 'Will Pay Soon\r\n', 1, NULL, '2026-03-05 14:56:30', NULL),
(10, 30, '2026-03-12', 'Call', 'wil pay next week', 1, NULL, '2026-03-12 06:29:15', NULL),
(11, 31, '2026-03-14', 'Call', 'pay tomorrow', 1, 1, '2026-03-13 17:47:59', '2026-03-13 17:50:43'),
(12, 32, '2026-03-14', 'Call', 'pay tomorrow', 1, NULL, '2026-03-14 04:59:29', NULL),
(13, 32, '2026-03-14', 'Call', 'pay tomorrow', 1, NULL, '2026-03-14 05:01:06', NULL),
(14, 33, '2026-03-14', 'Call', 'pay tomorrow', 1, NULL, '2026-03-14 05:06:48', NULL),
(15, 33, '2026-03-14', 'Email', 'pay tomorrow', 1, 1, '2026-03-14 05:08:13', '2026-03-14 05:09:02'),
(16, 35, '2026-03-14', 'Call', 'pay tomorrow', 1, NULL, '2026-03-14 05:19:46', NULL),
(17, 36, '2026-03-14', 'Call', 'pay tomorrow', 1, NULL, '2026-03-14 05:25:37', NULL),
(18, 36, '2026-03-14', 'Call', 'Pay tomorrow', 1, 1, '2026-03-14 05:27:21', '2026-03-14 05:28:18');

--
-- Triggers `tbl_follow_up`
--
DELIMITER $$
CREATE TRIGGER `trg_before_follow_up_insert` BEFORE INSERT ON `tbl_follow_up` FOR EACH ROW BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM tbl_admin WHERE employee_id = NEW.recorded_by AND is_active = 1
        UNION
        SELECT 1 FROM tbl_staff WHERE employee_id = NEW.recorded_by AND is_active = 1
        UNION
        SELECT 1 FROM tbl_collector WHERE employee_id = NEW.recorded_by AND is_active = 1
    ) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'You are not authorized for inserting follow up records.';
    END IF;
END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `trg_before_follow_up_update` BEFORE UPDATE ON `tbl_follow_up` FOR EACH ROW BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM tbl_admin WHERE employee_id = NEW.updated_by AND is_active = 1
        UNION
        SELECT 1 FROM tbl_staff WHERE employee_id = NEW.updated_by AND is_active = 1
        UNION
        SELECT 1 FROM tbl_collector WHERE employee_id = NEW.updated_by AND is_active = 1
    ) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'You are not authorized for updating follow up records.';
    END IF;
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table `tbl_installment_payment`
--

CREATE TABLE `tbl_installment_payment` (
  `payment_id` int(11) NOT NULL,
  `installment_id` int(11) NOT NULL,
  `transaction_id` int(11) NOT NULL,
  `payment_amount` decimal(12,2) NOT NULL,
  `recorded_by` int(11) NOT NULL,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `tbl_installment_payment`
--

INSERT INTO `tbl_installment_payment` (`payment_id`, `installment_id`, `transaction_id`, `payment_amount`, `recorded_by`, `created_at`) VALUES
(8, 14, 36, 1750.00, 2, '2026-02-11 15:21:06'),
(9, 15, 36, 750.00, 2, '2026-02-11 15:21:06'),
(10, 20, 37, 160.94, 2, '2026-02-11 15:21:06'),
(11, 21, 37, 160.94, 2, '2026-02-11 15:21:06'),
(12, 22, 37, 160.94, 2, '2026-02-11 15:21:06'),
(13, 23, 37, 160.94, 2, '2026-02-11 15:21:06'),
(14, 24, 37, 160.94, 2, '2026-02-11 15:21:06'),
(15, 25, 37, 160.94, 2, '2026-02-11 15:21:06'),
(16, 26, 37, 160.94, 2, '2026-02-11 15:21:06'),
(17, 27, 37, 73.42, 2, '2026-02-11 15:21:06'),
(18, 52, 38, 2687.50, 2, '2026-02-11 15:21:06'),
(19, 53, 38, 312.50, 2, '2026-02-11 15:21:06'),
(20, 58, 39, 5000.00, 2, '2026-02-11 15:21:06'),
(21, 62, 40, 10000.00, 2, '2026-02-11 15:21:06'),
(22, 15, 41, 500.00, 2, '2026-02-11 16:53:15'),
(23, 62, 42, 10000.00, 2, '2026-02-12 01:58:18'),
(24, 15, 43, 500.00, 2, '2026-02-12 04:41:32'),
(25, 16, 43, 500.00, 2, '2026-02-12 04:41:32'),
(26, 27, 44, 87.52, 2, '2026-02-24 03:42:41'),
(27, 28, 44, 160.94, 2, '2026-02-24 03:42:41'),
(28, 29, 44, 160.94, 2, '2026-02-24 03:42:41'),
(29, 30, 44, 90.60, 2, '2026-02-24 03:42:41'),
(30, 16, 45, 1250.00, 1, '2026-03-06 03:36:00'),
(31, 17, 45, 1750.00, 1, '2026-03-06 03:36:00'),
(32, 18, 45, 1750.00, 1, '2026-03-06 03:36:00'),
(33, 19, 45, 1750.00, 1, '2026-03-06 03:36:00'),
(34, 85, 46, 9000.00, 1, '2026-03-11 16:05:01'),
(35, 85, 47, 1500.00, 1, '2026-03-11 16:06:21'),
(36, 86, 47, 9500.00, 1, '2026-03-11 16:06:21'),
(37, 85, 48, 9000.00, 1, '2026-03-12 05:53:25'),
(38, 86, 48, 1000.00, 1, '2026-03-12 05:53:25'),
(39, 87, 49, 3000.00, 1, '2026-03-12 06:24:43'),
(40, 88, 49, 3000.00, 1, '2026-03-12 06:24:43'),
(41, 87, 50, 3000.00, 1, '2026-03-12 06:25:38'),
(42, 88, 50, 3000.00, 1, '2026-03-12 06:25:38'),
(43, 89, 51, 3000.00, 1, '2026-03-12 06:32:56'),
(44, 90, 51, 3000.00, 1, '2026-03-12 06:32:56'),
(45, 91, 51, 3000.00, 1, '2026-03-12 06:32:56'),
(46, 92, 51, 3000.00, 1, '2026-03-12 06:32:56'),
(47, 93, 51, 3000.00, 1, '2026-03-12 06:32:56'),
(48, 115, 52, 3000.00, 1, '2026-03-13 17:45:42'),
(49, 116, 52, 3000.00, 1, '2026-03-13 17:45:42'),
(50, 117, 52, 3000.00, 1, '2026-03-13 17:45:42'),
(51, 118, 52, 3000.00, 1, '2026-03-13 17:45:42'),
(52, 119, 52, 500.00, 1, '2026-03-13 17:45:42'),
(53, 115, 53, 3000.00, 1, '2026-03-13 17:46:47'),
(54, 116, 53, 3000.00, 1, '2026-03-13 17:46:47'),
(55, 117, 53, 3000.00, 1, '2026-03-13 17:46:47'),
(56, 118, 53, 3000.00, 1, '2026-03-13 17:46:47'),
(57, 119, 53, 500.00, 1, '2026-03-13 17:46:47'),
(58, 122, 54, 2250.00, 1, '2026-03-14 02:34:08'),
(59, 123, 54, 2250.00, 1, '2026-03-14 02:34:08'),
(60, 124, 54, 2250.00, 1, '2026-03-14 02:34:08'),
(61, 125, 54, 2250.00, 1, '2026-03-14 02:34:08'),
(62, 126, 54, 2250.00, 1, '2026-03-14 02:34:08'),
(63, 127, 54, 1250.00, 1, '2026-03-14 02:34:08'),
(64, 127, 55, 1000.00, 1, '2026-03-14 04:49:42'),
(65, 128, 55, 2250.00, 1, '2026-03-14 04:49:42'),
(66, 136, 56, 2000.00, 1, '2026-03-14 04:57:59'),
(67, 143, 57, 3000.00, 1, '2026-03-14 05:05:51'),
(68, 144, 57, 2000.00, 1, '2026-03-14 05:05:51'),
(69, 53, 58, 2375.00, 1, '2026-03-14 05:18:58'),
(70, 54, 58, 125.00, 1, '2026-03-14 05:18:58'),
(71, 62, 59, 15000.00, 1, '2026-03-14 05:24:48');

--
-- Triggers `tbl_installment_payment`
--
DELIMITER $$
CREATE TRIGGER `trg_after_payment_status_update` AFTER INSERT ON `tbl_installment_payment` FOR EACH ROW BEGIN
    DECLARE v_remaining_bal DECIMAL(12,2);
    DECLARE v_loan_id INT;

    
    SELECT total_amount_to_pay, loan_id
    INTO v_remaining_bal, v_loan_id
    FROM vw_total_amount_installment
    WHERE installment_id = NEW.installment_id
    LIMIT 1;

    
    IF v_remaining_bal <= 0 THEN

        UPDATE tbl_loan_installment
        SET installment_status = 'Paid',
            is_partially_paid = 0
        WHERE installment_id = NEW.installment_id;

        UPDATE tbl_past_due_account
        SET past_due_status = 'Resolved'
        WHERE installment_id = NEW.installment_id;

    ELSE

        UPDATE tbl_loan_installment
        SET is_partially_paid = 1
        WHERE installment_id = NEW.installment_id;

    END IF;

    
    IF NOT EXISTS (
        SELECT 1
        FROM tbl_loan_installment
        WHERE loan_id = v_loan_id
        AND (installment_status IS NULL OR installment_status <> 'Paid')
    ) THEN

        UPDATE tbl_loan
        SET loan_status = 'Paid'
        WHERE loan_id = v_loan_id;

    END IF;

END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `trg_update_statuses_on_payment` AFTER INSERT ON `tbl_installment_payment` FOR EACH ROW begin
declare v_new_bal decimal(12,2);
declare v_loan_id int;

select total_amount_to_pay, loan_id into v_new_bal, v_loan_id
from vw_total_amount_installment
where installment_id = new.installment_id;

if v_new_bal <= 0 then
update tbl_loan_installment set installment_status = 'Paid', is_partially_paid = 0 where installment_id = new.installment_id;
update tbl_past_due_account set past_due_status = 'Resolved' where installment_id = new.installment_id;
else
update tbl_loan_installment set is_partially_paid = 1 where installment_id = new.installment_id;
end if;

if not exists (select 1 from tbl_loan_installment where loan_id = v_loan_id and installment_status != 'Paid') then
update tbl_loan set loan_status = 'Paid' where loan_id = v_loan_id;
end if;
end
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table `tbl_loan`
--

CREATE TABLE `tbl_loan` (
  `loan_id` int(11) NOT NULL,
  `client_id` int(11) NOT NULL,
  `loan_amount` decimal(12,2) NOT NULL,
  `due_date` date NOT NULL,
  `installment_plan` int(11) NOT NULL,
  `interest_rate` decimal(5,2) NOT NULL,
  `loan_status` enum('Active','Paid','Past Due') NOT NULL DEFAULT 'Active',
  `approved_by` int(11) NOT NULL,
  `updated_by` int(11) DEFAULT NULL,
  `is_void` tinyint(1) NOT NULL DEFAULT 0,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp(),
  `updated_at` timestamp NULL DEFAULT NULL ON UPDATE current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `tbl_loan`
--

INSERT INTO `tbl_loan` (`loan_id`, `client_id`, `loan_amount`, `due_date`, `installment_plan`, `interest_rate`, `loan_status`, `approved_by`, `updated_by`, `is_void`, `created_at`, `updated_at`) VALUES
(6, 1, 10000.00, '2026-08-11', 30, 5.00, 'Paid', 1, NULL, 0, '2026-02-11 13:31:41', '2026-03-07 02:55:29'),
(7, 2, 5000.00, '2026-03-15', 1, 3.00, 'Active', 2, NULL, 0, '2026-02-11 13:31:41', '2026-03-07 02:55:29'),
(8, 3, 15000.00, '2026-05-11', 15, 7.50, 'Active', 1, NULL, 0, '2026-02-11 13:31:41', '2026-03-07 02:55:29'),
(9, 4, 25000.00, '2027-02-11', 90, 10.00, 'Active', 2, NULL, 0, '2026-02-11 13:31:41', '2026-03-07 02:55:29'),
(10, 5, 50000.00, '2027-02-11', 365, 12.00, 'Active', 1, NULL, 0, '2026-02-11 13:31:41', '2026-03-07 02:55:29'),
(12, 6, 10000.00, '2026-03-05', 1, 5.00, 'Active', 1, NULL, 1, '2026-03-05 14:27:55', '2026-03-07 02:55:29'),
(13, 6, 20000.00, '2026-03-09', 1, 5.00, 'Active', 1, NULL, 1, '2026-03-05 14:46:14', '2026-03-07 02:55:29'),
(14, 6, 20000.00, '2026-04-04', 15, 5.00, 'Active', 1, 1, 1, '2025-12-31 16:00:00', '2026-03-07 02:55:29'),
(15, 6, 20000.00, '2026-04-04', 15, 5.00, 'Active', 1, 1, 1, '2025-12-31 16:00:00', '2026-03-07 02:55:29'),
(16, 6, 20000.00, '2026-04-23', 20, 5.00, 'Paid', 1, NULL, 0, '2026-03-07 03:01:32', '2026-03-12 05:53:25'),
(17, 6, 20000.00, '2026-03-19', 1, 5.00, 'Paid', 1, NULL, 0, '2026-03-12 06:22:31', '2026-03-12 06:32:56'),
(18, 6, 5000.00, '2026-03-19', 1, 5.00, 'Active', 1, NULL, 1, '2026-03-12 06:33:18', '2026-03-14 02:19:27'),
(19, 7, 20000.00, '2026-03-21', 1, 5.00, 'Active', 1, NULL, 1, '2026-03-13 17:35:15', '2026-03-13 17:35:39'),
(20, 7, 20000.00, '2026-03-21', 1, 5.00, 'Active', 1, NULL, 1, '2026-03-13 17:35:58', '2026-03-13 17:44:20'),
(21, 7, 20000.00, '2026-03-21', 1, 5.00, 'Active', 1, NULL, 0, '2026-03-13 17:44:44', NULL),
(22, 6, 15000.00, '2026-03-21', 1, 5.00, 'Paid', 1, NULL, 0, '2026-03-14 02:20:43', '2026-03-14 04:49:42'),
(23, 6, 20000.00, '2026-03-21', 1, 5.00, 'Active', 1, NULL, 1, '2026-03-14 04:51:44', '2026-03-14 04:53:55'),
(24, 6, 20000.00, '2026-03-21', 1, 5.00, 'Active', 1, NULL, 1, '2026-03-14 04:55:22', '2026-03-14 05:03:57'),
(25, 6, 20000.00, '2026-03-21', 1, 5.00, 'Active', 1, NULL, 1, '2026-03-14 05:04:11', '2026-03-14 05:14:27'),
(26, 6, 20000.00, '2026-03-21', 1, 5.00, 'Active', 1, NULL, 1, '2026-03-14 05:17:05', '2026-03-14 05:17:16'),
(27, 6, 20000.00, '2026-03-21', 1, 5.00, 'Active', 1, NULL, 1, '2026-03-14 05:23:20', '2026-03-14 05:23:34');

--
-- Triggers `tbl_loan`
--
DELIMITER $$
CREATE TRIGGER `trg_auto_generate_installments_dynamic` AFTER INSERT ON `tbl_loan` FOR EACH ROW BEGIN
    DECLARE v_current_date DATE;
    DECLARE v_installment_count INT DEFAULT 0;
    DECLARE v_total_repayment DECIMAL(12,2);
    DECLARE v_per_installment_amount DECIMAL(12,2);
    DECLARE v_total_allocated DECIMAL(12,2) DEFAULT 0.00;
    DECLARE v_counter INT DEFAULT 0;

    
    SET v_current_date = CAST(NEW.created_at AS DATE);

    WHILE DATE_ADD(v_current_date, INTERVAL NEW.installment_plan DAY) <= NEW.due_date DO
        SET v_current_date = DATE_ADD(v_current_date, INTERVAL NEW.installment_plan DAY);
        SET v_installment_count = v_installment_count + 1;
    END WHILE;

    
    IF v_installment_count = 0 THEN
        SET v_installment_count = 1;
    END IF;

    SET v_total_repayment = NEW.loan_amount + (NEW.loan_amount * (NEW.interest_rate / 100));
    SET v_per_installment_amount = ROUND(v_total_repayment / v_installment_count, 2);

    SET v_current_date = CAST(NEW.created_at AS DATE);
    SET v_counter = 0;

    WHILE v_counter < v_installment_count DO
        SET v_counter = v_counter + 1;
        SET v_current_date = DATE_ADD(v_current_date, INTERVAL NEW.installment_plan DAY);

        
        IF v_current_date > NEW.due_date OR v_counter = v_installment_count THEN
            SET v_current_date = NEW.due_date;
        END IF;

        
        IF v_counter = v_installment_count THEN
            SET v_per_installment_amount = v_total_repayment - v_total_allocated;
        ELSE
            SET v_total_allocated = v_total_allocated + v_per_installment_amount;
        END IF;

        INSERT INTO tbl_loan_installment (
            loan_id,
            installment_amount,
            installment_due_date,
            installment_status,
            updated_by
        )
        VALUES (
            NEW.loan_id,
            v_per_installment_amount,
            v_current_date,
            'Active',
            NEW.approved_by
        );
    END WHILE;
END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `trg_before_loan_insert` BEFORE INSERT ON `tbl_loan` FOR EACH ROW begin
if not exists (
select 1 from tbl_admin where employee_id = new.approved_by and is_active = 1
union
select 1 from tbl_staff where employee_id = new.approved_by and is_active = 1
) then
signal sqlstate '45000'
set message_text = 'You are not authorize for inserting loan records.';
end if;
end
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `trg_before_loan_update` BEFORE UPDATE ON `tbl_loan` FOR EACH ROW begin
if not exists (
select 1 from tbl_admin where employee_id = new.approved_by and is_active = 1
union
select 1 from tbl_staff where employee_id = new.approved_by and is_active = 1
) then
signal sqlstate '45000'
set message_text = 'You are not authorize for updating loan records.';
end if;
end
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table `tbl_loan_installment`
--

CREATE TABLE `tbl_loan_installment` (
  `installment_id` int(11) NOT NULL,
  `loan_id` int(11) NOT NULL,
  `installment_amount` decimal(12,2) NOT NULL,
  `installment_due_date` date NOT NULL,
  `installment_status` enum('Active','Paid','Past Due') NOT NULL,
  `is_partially_paid` tinyint(1) NOT NULL DEFAULT 0,
  `updated_by` int(11) DEFAULT NULL,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp(),
  `updated_at` timestamp NULL DEFAULT NULL ON UPDATE current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `tbl_loan_installment`
--

INSERT INTO `tbl_loan_installment` (`installment_id`, `loan_id`, `installment_amount`, `installment_due_date`, `installment_status`, `is_partially_paid`, `updated_by`, `created_at`, `updated_at`) VALUES
(14, 6, 1750.00, '2026-03-11', 'Paid', 0, 1, '2026-02-11 13:31:41', NULL),
(15, 6, 1750.00, '2026-04-11', 'Paid', 0, 1, '2026-02-11 13:31:41', NULL),
(16, 6, 1750.00, '2026-05-11', 'Paid', 0, 1, '2026-02-11 13:31:41', '2026-03-06 03:36:00'),
(17, 6, 1750.00, '2026-06-11', 'Paid', 0, 1, '2026-02-11 13:31:41', '2026-03-06 03:36:00'),
(18, 6, 1750.00, '2026-07-11', 'Paid', 0, 1, '2026-02-11 13:31:41', '2026-03-06 03:36:00'),
(19, 6, 1750.00, '2026-08-11', 'Paid', 0, 1, '2026-02-11 13:31:41', '2026-03-06 03:36:00'),
(20, 7, 160.94, '2026-02-12', 'Paid', 0, 2, '2026-02-11 13:31:41', NULL),
(21, 7, 160.94, '2026-02-13', 'Paid', 0, 2, '2026-02-11 13:31:41', NULL),
(22, 7, 160.94, '2026-02-14', 'Paid', 0, 2, '2026-02-11 13:31:41', NULL),
(23, 7, 160.94, '2026-02-15', 'Paid', 0, 2, '2026-02-11 13:31:41', NULL),
(24, 7, 160.94, '2026-02-16', 'Paid', 0, 2, '2026-02-11 13:31:41', NULL),
(25, 7, 160.94, '2026-02-17', 'Paid', 0, 2, '2026-02-11 13:31:41', NULL),
(26, 7, 160.94, '2026-02-18', 'Paid', 0, 2, '2026-02-11 13:31:41', NULL),
(27, 7, 160.94, '2026-02-19', 'Past Due', 1, 2, '2026-02-11 13:31:41', '2026-03-05 16:15:40'),
(28, 7, 160.94, '2026-02-20', 'Past Due', 0, 2, '2026-02-11 13:31:41', '2026-03-05 16:15:40'),
(29, 7, 160.94, '2026-02-21', 'Past Due', 0, 2, '2026-02-11 13:31:41', '2026-03-05 16:15:40'),
(30, 7, 160.94, '2026-02-22', 'Past Due', 0, 2, '2026-02-11 13:31:41', '2026-03-05 16:15:40'),
(31, 7, 160.94, '2026-02-23', 'Past Due', 0, 2, '2026-02-11 13:31:41', NULL),
(32, 7, 160.94, '2026-02-24', 'Past Due', 0, 2, '2026-02-11 13:31:41', NULL),
(33, 7, 160.94, '2026-02-25', 'Past Due', 0, 2, '2026-02-11 13:31:41', NULL),
(34, 7, 160.94, '2026-02-26', 'Past Due', 0, 2, '2026-02-11 13:31:41', NULL),
(35, 7, 160.94, '2026-02-27', 'Past Due', 0, 2, '2026-02-11 13:31:41', NULL),
(36, 7, 160.94, '2026-02-28', 'Past Due', 0, 1, '2026-02-11 13:31:41', NULL),
(37, 7, 160.94, '2026-03-01', 'Past Due', 0, 1, '2026-02-11 13:31:41', NULL),
(38, 7, 160.94, '2026-03-02', 'Past Due', 0, 1, '2026-02-11 13:31:41', NULL),
(39, 7, 160.94, '2026-03-03', 'Past Due', 0, 1, '2026-02-11 13:31:41', NULL),
(40, 7, 160.94, '2026-03-04', 'Past Due', 0, 1, '2026-02-11 13:31:41', '2026-03-05 16:33:22'),
(41, 7, 160.94, '2026-03-05', 'Past Due', 0, 1, '2026-02-11 13:31:41', '2026-03-06 03:47:55'),
(42, 7, 160.94, '2026-03-06', 'Past Due', 0, 1, '2026-02-11 13:31:41', '2026-03-12 06:27:22'),
(43, 7, 160.94, '2026-03-07', 'Past Due', 0, 1, '2026-02-11 13:31:41', '2026-03-13 17:47:26'),
(44, 7, 160.94, '2026-03-08', 'Past Due', 0, 1, '2026-02-11 13:31:41', '2026-03-14 04:56:10'),
(45, 7, 160.94, '2026-03-09', 'Past Due', 0, 1, '2026-02-11 13:31:41', '2026-03-14 05:04:51'),
(46, 7, 160.94, '2026-03-10', 'Past Due', 0, 1, '2026-02-11 13:31:41', '2026-03-14 05:17:35'),
(47, 7, 160.94, '2026-03-11', 'Past Due', 0, 1, '2026-02-11 13:31:41', '2026-03-14 05:23:59'),
(48, 7, 160.94, '2026-03-12', 'Active', 0, 2, '2026-02-11 13:31:41', NULL),
(49, 7, 160.94, '2026-03-13', 'Active', 0, 2, '2026-02-11 13:31:41', NULL),
(50, 7, 160.94, '2026-03-14', 'Active', 0, 2, '2026-02-11 13:31:41', NULL),
(51, 7, 160.86, '2026-03-15', 'Active', 0, 2, '2026-02-11 13:31:41', NULL),
(52, 8, 2687.50, '2026-02-26', 'Paid', 0, 1, '2026-02-11 13:31:41', NULL),
(53, 8, 2687.50, '2026-03-13', 'Past Due', 1, 1, '2026-02-11 13:31:41', '2026-03-14 05:19:12'),
(54, 8, 2687.50, '2026-03-28', 'Active', 0, 1, '2026-02-11 13:31:41', '2026-03-14 05:19:12'),
(55, 8, 2687.50, '2026-04-12', 'Active', 0, 1, '2026-02-11 13:31:41', NULL),
(56, 8, 2687.50, '2026-04-27', 'Active', 0, 1, '2026-02-11 13:31:41', NULL),
(57, 8, 2687.50, '2026-05-11', 'Active', 0, 1, '2026-02-11 13:31:41', NULL),
(58, 9, 6875.00, '2026-05-11', 'Active', 1, 2, '2026-02-11 13:31:41', NULL),
(59, 9, 6875.00, '2026-08-11', 'Active', 0, 2, '2026-02-11 13:31:41', NULL),
(60, 9, 6875.00, '2026-11-11', 'Active', 0, 2, '2026-02-11 13:31:41', NULL),
(61, 9, 6875.00, '2027-02-11', 'Active', 0, 2, '2026-02-11 13:31:41', NULL),
(62, 10, 56000.00, '2027-02-11', 'Active', 1, 1, '2026-02-11 13:31:41', NULL),
(67, 13, 5250.00, '2026-03-06', 'Active', 0, 1, '2026-03-05 14:46:14', NULL),
(68, 13, 5250.00, '2026-03-07', 'Active', 0, 1, '2026-03-05 14:46:14', NULL),
(69, 13, 5250.00, '2026-03-08', 'Active', 0, 1, '2026-03-05 14:46:14', NULL),
(70, 13, 5250.00, '2026-03-09', 'Active', 0, 1, '2026-03-05 14:46:14', NULL),
(71, 14, 3000.00, '2026-01-16', 'Past Due', 0, 1, '2026-03-05 14:53:09', NULL),
(72, 14, 3000.00, '2026-01-31', 'Past Due', 0, 1, '2026-03-05 14:53:09', NULL),
(73, 14, 3000.00, '2026-02-15', 'Past Due', 0, 1, '2026-03-05 14:53:09', NULL),
(74, 14, 3000.00, '2026-03-02', 'Past Due', 0, 1, '2026-03-05 14:53:09', NULL),
(75, 14, 3000.00, '2026-03-17', 'Active', 0, 1, '2026-03-05 14:53:09', NULL),
(76, 14, 3000.00, '2026-04-01', 'Active', 0, 1, '2026-03-05 14:53:09', NULL),
(77, 14, 3000.00, '2026-04-04', 'Active', 0, 1, '2026-03-05 14:53:09', NULL),
(78, 15, 3000.00, '2026-01-16', 'Past Due', 0, 1, '2026-03-05 14:55:44', NULL),
(79, 15, 3000.00, '2026-01-31', 'Active', 0, 1, '2026-03-05 14:55:44', NULL),
(80, 15, 3000.00, '2026-02-15', 'Active', 0, 1, '2026-03-05 14:55:44', NULL),
(81, 15, 3000.00, '2026-03-02', 'Active', 0, 1, '2026-03-05 14:55:44', NULL),
(82, 15, 3000.00, '2026-03-17', 'Active', 0, 1, '2026-03-05 14:55:44', NULL),
(83, 15, 3000.00, '2026-04-01', 'Active', 0, 1, '2026-03-05 14:55:44', NULL),
(84, 15, 3000.00, '2026-04-04', 'Active', 0, 1, '2026-03-05 14:55:44', NULL),
(85, 16, 10500.00, '2026-03-27', 'Paid', 0, 1, '2026-03-07 03:01:32', '2026-03-12 05:53:25'),
(86, 16, 10500.00, '2026-04-23', 'Paid', 0, 1, '2026-03-07 03:01:32', '2026-03-12 05:53:25'),
(87, 17, 3000.00, '2026-03-13', 'Paid', 0, 1, '2026-03-12 06:22:31', '2026-03-12 06:25:38'),
(88, 17, 3000.00, '2026-03-14', 'Paid', 0, 1, '2026-03-12 06:22:31', '2026-03-12 06:25:38'),
(89, 17, 3000.00, '2026-03-15', 'Paid', 0, 1, '2026-03-12 06:22:31', '2026-03-12 06:32:56'),
(90, 17, 3000.00, '2026-03-16', 'Paid', 0, 1, '2026-03-12 06:22:31', '2026-03-12 06:32:56'),
(91, 17, 3000.00, '2026-03-17', 'Paid', 0, 1, '2026-03-12 06:22:31', '2026-03-12 06:32:56'),
(92, 17, 3000.00, '2026-03-18', 'Paid', 0, 1, '2026-03-12 06:22:31', '2026-03-12 06:32:56'),
(93, 17, 3000.00, '2026-03-19', 'Paid', 0, 1, '2026-03-12 06:22:31', '2026-03-12 06:32:56'),
(94, 18, 750.00, '2026-03-13', 'Active', 0, 1, '2026-03-12 06:33:18', NULL),
(95, 18, 750.00, '2026-03-14', 'Active', 0, 1, '2026-03-12 06:33:18', NULL),
(96, 18, 750.00, '2026-03-15', 'Active', 0, 1, '2026-03-12 06:33:18', NULL),
(97, 18, 750.00, '2026-03-16', 'Active', 0, 1, '2026-03-12 06:33:18', NULL),
(98, 18, 750.00, '2026-03-17', 'Active', 0, 1, '2026-03-12 06:33:18', NULL),
(99, 18, 750.00, '2026-03-18', 'Active', 0, 1, '2026-03-12 06:33:18', NULL),
(100, 18, 750.00, '2026-03-19', 'Active', 0, 1, '2026-03-12 06:33:18', NULL),
(101, 19, 3000.00, '2026-03-15', 'Active', 0, 1, '2026-03-13 17:35:15', NULL),
(102, 19, 3000.00, '2026-03-16', 'Active', 0, 1, '2026-03-13 17:35:15', NULL),
(103, 19, 3000.00, '2026-03-17', 'Active', 0, 1, '2026-03-13 17:35:15', NULL),
(104, 19, 3000.00, '2026-03-18', 'Active', 0, 1, '2026-03-13 17:35:15', NULL),
(105, 19, 3000.00, '2026-03-19', 'Active', 0, 1, '2026-03-13 17:35:15', NULL),
(106, 19, 3000.00, '2026-03-20', 'Active', 0, 1, '2026-03-13 17:35:15', NULL),
(107, 19, 3000.00, '2026-03-21', 'Active', 0, 1, '2026-03-13 17:35:15', NULL),
(108, 20, 3000.00, '2026-03-15', 'Active', 0, 1, '2026-03-13 17:35:58', NULL),
(109, 20, 3000.00, '2026-03-16', 'Active', 0, 1, '2026-03-13 17:35:58', NULL),
(110, 20, 3000.00, '2026-03-17', 'Active', 0, 1, '2026-03-13 17:35:58', NULL),
(111, 20, 3000.00, '2026-03-18', 'Active', 0, 1, '2026-03-13 17:35:58', NULL),
(112, 20, 3000.00, '2026-03-19', 'Active', 0, 1, '2026-03-13 17:35:58', NULL),
(113, 20, 3000.00, '2026-03-20', 'Active', 0, 1, '2026-03-13 17:35:58', NULL),
(114, 20, 3000.00, '2026-03-21', 'Active', 0, 1, '2026-03-13 17:35:58', NULL),
(115, 21, 3000.00, '2026-03-15', 'Paid', 0, 1, '2026-03-13 17:44:44', '2026-03-13 17:46:47'),
(116, 21, 3000.00, '2026-03-16', 'Paid', 0, 1, '2026-03-13 17:44:44', '2026-03-13 17:46:47'),
(117, 21, 3000.00, '2026-03-17', 'Paid', 0, 1, '2026-03-13 17:44:44', '2026-03-13 17:46:47'),
(118, 21, 3000.00, '2026-03-18', 'Paid', 0, 1, '2026-03-13 17:44:44', '2026-03-13 17:46:47'),
(119, 21, 3000.00, '2026-03-19', 'Active', 1, 1, '2026-03-13 17:44:44', '2026-03-13 17:46:47'),
(120, 21, 3000.00, '2026-03-20', 'Active', 0, 1, '2026-03-13 17:44:44', NULL),
(121, 21, 3000.00, '2026-03-21', 'Active', 0, 1, '2026-03-13 17:44:44', NULL),
(122, 22, 2250.00, '2026-03-15', 'Paid', 0, 1, '2026-03-14 02:20:43', '2026-03-14 02:34:08'),
(123, 22, 2250.00, '2026-03-16', 'Paid', 0, 1, '2026-03-14 02:20:43', '2026-03-14 02:34:08'),
(124, 22, 2250.00, '2026-03-17', 'Paid', 0, 1, '2026-03-14 02:20:43', '2026-03-14 02:34:08'),
(125, 22, 2250.00, '2026-03-18', 'Paid', 0, 1, '2026-03-14 02:20:43', '2026-03-14 02:34:08'),
(126, 22, 2250.00, '2026-03-19', 'Paid', 0, 1, '2026-03-14 02:20:43', '2026-03-14 02:34:08'),
(127, 22, 2250.00, '2026-03-20', 'Paid', 0, 1, '2026-03-14 02:20:43', '2026-03-14 04:49:42'),
(128, 22, 2250.00, '2026-03-21', 'Paid', 0, 1, '2026-03-14 02:20:43', '2026-03-14 04:49:42'),
(129, 23, 3000.00, '2026-03-15', 'Active', 0, 1, '2026-03-14 04:51:44', NULL),
(130, 23, 3000.00, '2026-03-16', 'Active', 0, 1, '2026-03-14 04:51:44', NULL),
(131, 23, 3000.00, '2026-03-17', 'Active', 0, 1, '2026-03-14 04:51:44', NULL),
(132, 23, 3000.00, '2026-03-18', 'Active', 0, 1, '2026-03-14 04:51:44', NULL),
(133, 23, 3000.00, '2026-03-19', 'Active', 0, 1, '2026-03-14 04:51:44', NULL),
(134, 23, 3000.00, '2026-03-20', 'Active', 0, 1, '2026-03-14 04:51:44', NULL),
(135, 23, 3000.00, '2026-03-21', 'Active', 0, 1, '2026-03-14 04:51:44', NULL),
(136, 24, 3000.00, '2026-03-15', 'Active', 0, 1, '2026-03-14 04:55:22', '2026-03-14 04:58:17'),
(137, 24, 3000.00, '2026-03-16', 'Active', 0, 1, '2026-03-14 04:55:22', NULL),
(138, 24, 3000.00, '2026-03-17', 'Active', 0, 1, '2026-03-14 04:55:22', NULL),
(139, 24, 3000.00, '2026-03-18', 'Active', 0, 1, '2026-03-14 04:55:22', NULL),
(140, 24, 3000.00, '2026-03-19', 'Active', 0, 1, '2026-03-14 04:55:22', NULL),
(141, 24, 3000.00, '2026-03-20', 'Active', 0, 1, '2026-03-14 04:55:22', NULL),
(142, 24, 3000.00, '2026-03-21', 'Active', 0, 1, '2026-03-14 04:55:22', NULL),
(143, 25, 3000.00, '2026-03-15', 'Active', 0, 1, '2026-03-14 05:04:11', '2026-03-14 05:06:08'),
(144, 25, 3000.00, '2026-03-16', 'Active', 0, 1, '2026-03-14 05:04:11', '2026-03-14 05:06:08'),
(145, 25, 3000.00, '2026-03-17', 'Active', 0, 1, '2026-03-14 05:04:11', NULL),
(146, 25, 3000.00, '2026-03-18', 'Active', 0, 1, '2026-03-14 05:04:11', NULL),
(147, 25, 3000.00, '2026-03-19', 'Active', 0, 1, '2026-03-14 05:04:11', NULL),
(148, 25, 3000.00, '2026-03-20', 'Active', 0, 1, '2026-03-14 05:04:11', NULL),
(149, 25, 3000.00, '2026-03-21', 'Active', 0, 1, '2026-03-14 05:04:11', NULL),
(150, 26, 3000.00, '2026-03-15', 'Active', 0, 1, '2026-03-14 05:17:05', NULL),
(151, 26, 3000.00, '2026-03-16', 'Active', 0, 1, '2026-03-14 05:17:05', NULL),
(152, 26, 3000.00, '2026-03-17', 'Active', 0, 1, '2026-03-14 05:17:05', NULL),
(153, 26, 3000.00, '2026-03-18', 'Active', 0, 1, '2026-03-14 05:17:05', NULL),
(154, 26, 3000.00, '2026-03-19', 'Active', 0, 1, '2026-03-14 05:17:05', NULL),
(155, 26, 3000.00, '2026-03-20', 'Active', 0, 1, '2026-03-14 05:17:05', NULL),
(156, 26, 3000.00, '2026-03-21', 'Active', 0, 1, '2026-03-14 05:17:05', NULL),
(157, 27, 3000.00, '2026-03-15', 'Active', 0, 1, '2026-03-14 05:23:20', NULL),
(158, 27, 3000.00, '2026-03-16', 'Active', 0, 1, '2026-03-14 05:23:20', NULL),
(159, 27, 3000.00, '2026-03-17', 'Active', 0, 1, '2026-03-14 05:23:20', NULL),
(160, 27, 3000.00, '2026-03-18', 'Active', 0, 1, '2026-03-14 05:23:20', NULL),
(161, 27, 3000.00, '2026-03-19', 'Active', 0, 1, '2026-03-14 05:23:20', NULL),
(162, 27, 3000.00, '2026-03-20', 'Active', 0, 1, '2026-03-14 05:23:20', NULL),
(163, 27, 3000.00, '2026-03-21', 'Active', 0, 1, '2026-03-14 05:23:20', NULL);

--
-- Triggers `tbl_loan_installment`
--
DELIMITER $$
CREATE TRIGGER `trg_after_installment_past_due` AFTER UPDATE ON `tbl_loan_installment` FOR EACH ROW BEGIN
    
    IF NEW.installment_status = 'Past Due' AND OLD.installment_status <> 'Past Due' THEN
        INSERT INTO tbl_past_due_account (
            installment_id,
            past_due_status,
            penalty_added,
            added_by,
            created_at
        ) VALUES (
            NEW.installment_id,
            'Open',
            0.00,
            NEW.updated_by,
            NOW()
        );

        
        
        IF NOT EXISTS (
            SELECT 1
            FROM tbl_loan_installment
            WHERE loan_id = NEW.loan_id
            AND installment_status <> 'Past Due'
        ) THEN
            UPDATE tbl_loan
            SET loan_status = 'Past Due'
            WHERE loan_id = NEW.loan_id;
        END IF;
    END IF;
END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `trg_before_installment_update` BEFORE UPDATE ON `tbl_loan_installment` FOR EACH ROW BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM tbl_admin WHERE employee_id = NEW.updated_by AND is_active = 1
        UNION
        SELECT 1 FROM tbl_staff WHERE employee_id = NEW.updated_by AND is_active = 1
        UNION
        SELECT 1 FROM tbl_collector WHERE employee_id = NEW.updated_by AND is_active = 1
    ) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'You are not authorized for updating installment records.';
    END IF;
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table `tbl_past_due_account`
--

CREATE TABLE `tbl_past_due_account` (
  `past_due_id` int(11) NOT NULL,
  `installment_id` int(11) NOT NULL,
  `past_due_status` enum('Open','Promise to Pay','Resolved') NOT NULL DEFAULT 'Open',
  `penalty_added` decimal(12,2) NOT NULL,
  `added_by` int(11) NOT NULL,
  `updated_by` int(11) DEFAULT NULL,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `tbl_past_due_account`
--

INSERT INTO `tbl_past_due_account` (`past_due_id`, `installment_id`, `past_due_status`, `penalty_added`, `added_by`, `updated_by`, `created_at`) VALUES
(6, 20, 'Resolved', 0.00, 2, 2, '2026-02-11 13:45:13'),
(7, 21, 'Resolved', 0.00, 2, 2, '2026-02-11 13:45:13'),
(8, 22, 'Resolved', 0.00, 2, 2, '2026-02-11 13:45:13'),
(9, 15, 'Resolved', 0.00, 1, 2, '2026-02-12 04:24:38'),
(10, 30, 'Promise to Pay', 50.00, 2, 1, '2026-02-24 05:02:49'),
(11, 31, 'Promise to Pay', 70.00, 2, 1, '2026-02-24 05:02:49'),
(12, 32, 'Open', 0.00, 2, NULL, '2026-02-28 08:53:08'),
(13, 33, 'Open', 0.00, 2, NULL, '2026-02-28 08:53:10'),
(14, 34, 'Open', 0.00, 2, NULL, '2026-02-28 08:53:12'),
(15, 35, 'Open', 50.00, 2, 1, '2026-02-28 08:53:14'),
(16, 36, 'Open', 70.00, 1, 1, '2026-02-28 16:27:54'),
(17, 37, 'Open', 100.00, 1, 1, '2026-03-05 02:00:15'),
(18, 38, 'Open', 100.00, 1, 1, '2026-03-05 02:00:36'),
(19, 39, 'Open', 200.00, 1, 1, '2026-03-05 02:00:43'),
(20, 71, 'Open', 50.00, 1, 1, '2026-03-05 14:53:28'),
(21, 72, 'Open', 50.00, 1, 1, '2026-03-05 14:53:31'),
(22, 73, 'Open', 50.00, 1, 1, '2026-03-05 14:53:35'),
(23, 74, 'Open', 100.00, 1, 1, '2026-03-05 14:53:40'),
(24, 78, 'Promise to Pay', 250.00, 1, 1, '2026-03-05 14:56:01'),
(25, 27, 'Open', 0.00, 2, NULL, '2026-03-05 16:15:40'),
(26, 28, 'Open', 0.00, 2, NULL, '2026-03-05 16:15:40'),
(27, 29, 'Open', 0.00, 2, NULL, '2026-03-05 16:15:40'),
(28, 40, 'Open', 100.00, 1, 1, '2026-03-05 16:33:22'),
(29, 41, 'Open', 50.00, 1, 1, '2026-03-06 03:47:55'),
(30, 42, 'Promise to Pay', 500.00, 1, 1, '2026-03-12 06:27:22'),
(31, 43, 'Promise to Pay', 250.00, 1, 1, '2026-03-13 17:47:26'),
(32, 44, 'Promise to Pay', 200.00, 1, 1, '2026-03-14 04:56:10'),
(33, 45, 'Promise to Pay', 250.00, 1, 1, '2026-03-14 05:04:51'),
(34, 46, 'Open', 100.00, 1, 1, '2026-03-14 05:17:35'),
(35, 53, 'Promise to Pay', 500.00, 1, 1, '2026-03-14 05:19:12'),
(36, 47, 'Promise to Pay', 100.00, 1, 1, '2026-03-14 05:23:59');

--
-- Triggers `tbl_past_due_account`
--
DELIMITER $$
CREATE TRIGGER `trg_before_past_due_account_insert` BEFORE INSERT ON `tbl_past_due_account` FOR EACH ROW begin
if not exists (
select 1 from tbl_admin where employee_id = new.added_by and is_active = 1
union
select 1 from tbl_staff where employee_id = new.added_by and is_active = 1
) then
signal sqlstate '45000'
set message_text = 'You are not authorize for inserting past due account records.';
end if;
end
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `trg_before_past_due_account_update` BEFORE UPDATE ON `tbl_past_due_account` FOR EACH ROW begin
if not exists (
select 1 from tbl_admin where employee_id = new.added_by and is_active = 1
union
select 1 from tbl_staff where employee_id = new.added_by and is_active = 1
) then
signal sqlstate '45000'
set message_text = 'You are not authorize for updating past due account records.';
end if;
end
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table `tbl_promise`
--

CREATE TABLE `tbl_promise` (
  `promise_id` int(11) NOT NULL,
  `past_due_id` int(11) NOT NULL,
  `promise_amount` decimal(12,2) NOT NULL,
  `promise_payment_date` date NOT NULL,
  `remarks` text DEFAULT NULL,
  `recorded_by` int(11) NOT NULL,
  `updated_by` int(11) DEFAULT NULL,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp(),
  `updated_at` timestamp NULL DEFAULT NULL ON UPDATE current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `tbl_promise`
--

INSERT INTO `tbl_promise` (`promise_id`, `past_due_id`, `promise_amount`, `promise_payment_date`, `remarks`, `recorded_by`, `updated_by`, `created_at`, `updated_at`) VALUES
(14, 6, 160.94, '2026-02-18', 'Client promised to pay full amount next week.', 2, 1, '2026-02-11 14:17:10', '2026-03-05 16:30:51'),
(15, 7, 160.94, '2026-02-20', 'Settlement after payday.', 2, NULL, '2026-02-11 14:17:10', NULL),
(16, 8, 160.94, '2026-02-25', 'Client will visit the office to pay.', 2, NULL, '2026-02-11 14:17:10', NULL),
(17, 9, 500.00, '2026-04-28', 'Will be payed on 2026-04-28', 2, NULL, '2026-02-12 04:32:34', NULL),
(19, 11, 230.94, '2026-03-09', 'i will pay tomorrow', 1, NULL, '2026-03-01 17:20:54', NULL),
(20, 10, 210.94, '2026-03-09', 'daad', 1, 1, '2026-03-01 17:30:58', '2026-03-05 16:41:07'),
(21, 24, 3200.00, '2026-03-12', 'Soon', 1, NULL, '2026-03-05 14:56:47', NULL),
(22, 30, 410.94, '2026-03-19', 'promise to pay at that date', 1, 1, '2026-03-12 06:29:57', '2026-03-12 06:54:48'),
(23, 31, 410.94, '2026-03-15', 'pay tomorrow', 1, 1, '2026-03-13 17:48:23', '2026-03-13 17:50:58'),
(24, 32, 360.94, '2026-03-21', 'pay tomorrow\r\n', 1, NULL, '2026-03-14 04:59:49', NULL),
(25, 32, 360.94, '2026-03-21', 'pay tomorrow\r\n', 1, NULL, '2026-03-14 05:01:17', NULL),
(26, 33, 250.94, '2026-03-21', 'pay tomorrow', 1, NULL, '2026-03-14 05:07:06', NULL),
(27, 33, 410.94, '2026-03-21', 'pay next week', 1, 1, '2026-03-14 05:08:23', '2026-03-14 05:09:25'),
(28, 35, 2500.00, '2026-03-15', 'pay tomorrw', 1, NULL, '2026-03-14 05:20:02', NULL),
(29, 36, 260.94, '2026-03-15', 'pay tomorrow', 1, NULL, '2026-03-14 05:25:54', NULL),
(30, 36, 260.94, '2026-03-15', 'Pay tomorrow', 1, 1, '2026-03-14 05:27:37', '2026-03-14 05:28:51');

--
-- Triggers `tbl_promise`
--
DELIMITER $$
CREATE TRIGGER `tr_before_promise_insert` BEFORE INSERT ON `tbl_promise` FOR EACH ROW BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM tbl_admin WHERE employee_id = NEW.recorded_by AND is_active = 1
        UNION
        SELECT 1 FROM tbl_staff WHERE employee_id = NEW.recorded_by AND is_active = 1
        UNION
        SELECT 1 FROM tbl_collector WHERE employee_id = NEW.recorded_by AND is_active = 1
    ) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'You are not authorized for inserting promise records.';
    END IF;
END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `trg_after_promise_insert` AFTER INSERT ON `tbl_promise` FOR EACH ROW begin
update tbl_past_due_account set past_due_status = 'Promise to Pay', updated_by = new.recorded_by where past_due_id = new.past_due_id;
end
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `trg_before_promise_update` BEFORE UPDATE ON `tbl_promise` FOR EACH ROW BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM tbl_admin WHERE employee_id = NEW.updated_by AND is_active = 1
        UNION
        SELECT 1 FROM tbl_staff WHERE employee_id = NEW.updated_by AND is_active = 1
        UNION
        SELECT 1 FROM tbl_collector WHERE employee_id = NEW.updated_by AND is_active = 1
    ) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'You are not authorized for updating promise records.';
    END IF;
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table `tbl_staff`
--

CREATE TABLE `tbl_staff` (
  `staff_id` int(11) NOT NULL,
  `employee_id` int(11) NOT NULL,
  `is_active` tinyint(1) NOT NULL DEFAULT 1,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `tbl_staff`
--

INSERT INTO `tbl_staff` (`staff_id`, `employee_id`, `is_active`, `created_at`) VALUES
(1, 2, 1, '2026-02-11 06:55:26'),
(2, 4, 1, '2026-02-11 06:55:26'),
(3, 7, 1, '2026-03-04 15:03:13'),
(4, 8, 0, '2026-03-13 17:53:53'),
(5, 12, 0, '2026-03-14 05:31:33');

-- --------------------------------------------------------

--
-- Table structure for table `tbl_transaction`
--

CREATE TABLE `tbl_transaction` (
  `transaction_id` int(11) NOT NULL,
  `client_id` int(11) NOT NULL,
  `transaction_type` enum('Cash','Back Transfer','GCash','Check') NOT NULL,
  `transaction_amount` decimal(12,2) NOT NULL,
  `status` enum('Confirmed','Void') DEFAULT NULL,
  `recorded_by` int(11) NOT NULL,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp(),
  `updated_by` int(11) DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL ON UPDATE current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `tbl_transaction`
--

INSERT INTO `tbl_transaction` (`transaction_id`, `client_id`, `transaction_type`, `transaction_amount`, `status`, `recorded_by`, `created_at`, `updated_by`, `updated_at`) VALUES
(36, 1, 'Cash', 2500.00, 'Confirmed', 2, '2026-02-11 15:21:06', NULL, NULL),
(37, 2, 'GCash', 1200.00, 'Confirmed', 2, '2026-02-11 15:21:06', NULL, NULL),
(38, 3, 'Cash', 3000.00, 'Confirmed', 2, '2026-02-11 15:21:06', NULL, NULL),
(39, 4, 'Cash', 5000.00, 'Confirmed', 2, '2026-02-11 15:21:06', NULL, NULL),
(40, 5, 'GCash', 10000.00, 'Confirmed', 2, '2026-02-11 15:21:06', NULL, NULL),
(41, 1, 'Cash', 500.00, 'Confirmed', 2, '2026-02-11 16:53:15', NULL, NULL),
(42, 5, 'GCash', 10000.00, 'Confirmed', 2, '2026-02-12 01:58:18', NULL, NULL),
(43, 1, 'Cash', 1000.00, 'Confirmed', 2, '2026-02-12 04:41:32', NULL, NULL),
(44, 2, 'Cash', 500.00, 'Void', 2, '2026-02-24 03:42:41', NULL, '2026-03-05 16:15:40'),
(45, 1, 'Cash', 6500.00, 'Confirmed', 1, '2026-03-06 03:36:00', NULL, NULL),
(46, 6, 'Cash', 9000.00, 'Void', 1, '2026-03-11 16:05:01', NULL, '2026-03-11 16:06:39'),
(47, 6, 'GCash', 11000.00, 'Confirmed', 1, '2026-03-11 16:06:21', NULL, NULL),
(48, 6, 'Back Transfer', 10000.00, 'Confirmed', 1, '2026-03-12 05:53:25', NULL, NULL),
(49, 6, 'Cash', 6000.00, 'Void', 1, '2026-03-12 06:24:43', NULL, '2026-03-12 06:25:21'),
(50, 6, 'Cash', 6000.00, 'Confirmed', 1, '2026-03-12 06:25:38', NULL, NULL),
(51, 6, 'Cash', 15000.00, 'Confirmed', 1, '2026-03-12 06:32:56', NULL, NULL),
(52, 7, 'GCash', 12500.00, 'Void', 1, '2026-03-13 17:45:42', NULL, '2026-03-13 17:46:26'),
(53, 7, 'Back Transfer', 12500.00, 'Confirmed', 1, '2026-03-13 17:46:47', NULL, NULL),
(54, 6, 'GCash', 12500.00, 'Confirmed', 1, '2026-03-14 02:34:08', NULL, NULL),
(55, 6, 'Cash', 3250.00, 'Confirmed', 1, '2026-03-14 04:49:42', NULL, NULL),
(56, 6, 'Back Transfer', 2000.00, 'Void', 1, '2026-03-14 04:57:59', NULL, '2026-03-14 04:58:17'),
(57, 6, 'Cash', 5000.00, 'Void', 1, '2026-03-14 05:05:51', NULL, '2026-03-14 05:06:08'),
(58, 3, 'Back Transfer', 2500.00, 'Void', 1, '2026-03-14 05:18:58', NULL, '2026-03-14 05:19:12'),
(59, 5, 'Back Transfer', 15000.00, 'Void', 1, '2026-03-14 05:24:48', NULL, '2026-03-14 05:25:02');

--
-- Triggers `tbl_transaction`
--
DELIMITER $$
CREATE TRIGGER `trg_after_transaction_allocation` AFTER INSERT ON `tbl_transaction` FOR EACH ROW begin
call sp_execute_waterfall_payment(
new.transaction_id,
new.client_id,
new.transaction_amount,
new.recorded_by
);
end
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `trg_after_transaction_status_change` AFTER UPDATE ON `tbl_transaction` FOR EACH ROW BEGIN
    
    IF NEW.status = 'Void' AND OLD.status = 'Confirmed' THEN

        
        UPDATE tbl_loan_installment li
        JOIN (
            SELECT
                li2.installment_id,
                IFNULL(
                    SUM(
                        CASE 
                            WHEN t2.status = 'Confirmed'
                                 AND t2.transaction_id != NEW.transaction_id
                            THEN ip2.payment_amount
                            ELSE 0
                        END
                    ), 
                    0
                ) AS remaining_confirmed
            FROM tbl_loan_installment li2
            LEFT JOIN tbl_installment_payment ip2 
                ON li2.installment_id = ip2.installment_id
            LEFT JOIN tbl_transaction t2 
                ON ip2.transaction_id = t2.transaction_id
            WHERE li2.installment_id IN (
                SELECT installment_id 
                FROM tbl_installment_payment 
                WHERE transaction_id = NEW.transaction_id
            )
            GROUP BY li2.installment_id
        ) calc 
            ON li.installment_id = calc.installment_id
        SET
            li.installment_status = IF(li.installment_due_date < CURDATE(), 'Past Due', 'Active'),
            li.is_partially_paid = IF(calc.remaining_confirmed > 0, 1, 0)
        WHERE li.installment_id IN (
            SELECT installment_id 
            FROM tbl_installment_payment 
            WHERE transaction_id = NEW.transaction_id
        );

        
        UPDATE tbl_loan l
        JOIN tbl_loan_installment li 
            ON l.loan_id = li.loan_id
        SET l.loan_status = 'Active'
        WHERE li.installment_id IN (
            SELECT installment_id 
            FROM tbl_installment_payment 
            WHERE transaction_id = NEW.transaction_id
        )
        AND l.loan_status = 'Paid';

    END IF;

    
    IF NEW.status = 'Confirmed' AND OLD.status = 'Void' THEN

        
        UPDATE tbl_loan_installment li
        JOIN (
            SELECT
                vw.installment_id,
                vw.total_amount_to_pay,
                IFNULL(
                    SUM(
                        CASE 
                            WHEN t2.status = 'Confirmed'
                                 OR t2.transaction_id = NEW.transaction_id
                            THEN ip2.payment_amount
                            ELSE 0
                        END
                    ),
                    0
                ) AS total_confirmed
            FROM vw_total_amount_installment vw
            LEFT JOIN tbl_installment_payment ip2 
                ON vw.installment_id = ip2.installment_id
            LEFT JOIN tbl_transaction t2 
                ON ip2.transaction_id = t2.transaction_id
            WHERE vw.installment_id IN (
                SELECT installment_id 
                FROM tbl_installment_payment 
                WHERE transaction_id = NEW.transaction_id
            )
            GROUP BY vw.installment_id
        ) calc 
            ON li.installment_id = calc.installment_id
        SET
            li.installment_status = IF(calc.total_amount_to_pay <= 0, 'Paid', li.installment_status),
            li.is_partially_paid = IF(
                calc.total_amount_to_pay <= 0,
                0,
                IF(calc.total_confirmed > 0, 1, 0)
            )
        WHERE li.installment_id IN (
            SELECT installment_id 
            FROM tbl_installment_payment 
            WHERE transaction_id = NEW.transaction_id
        );

        
        UPDATE tbl_loan l
        SET l.loan_status = 'Paid'
        WHERE l.loan_id IN (
            SELECT loan_id 
            FROM tbl_loan_installment 
            WHERE installment_id IN (
                SELECT installment_id 
                FROM tbl_installment_payment 
                WHERE transaction_id = NEW.transaction_id
            )
        )
        AND NOT EXISTS (
            SELECT 1 
            FROM tbl_loan_installment
            WHERE loan_id = l.loan_id 
              AND installment_status != 'Paid'
        );

    END IF;

END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `trg_after_transaction_void` AFTER UPDATE ON `tbl_transaction` FOR EACH ROW BEGIN
    
    IF NEW.status = 'Void' AND OLD.status = 'Confirmed' THEN

        
        UPDATE tbl_loan_installment li
        JOIN tbl_installment_payment ip 
            ON li.installment_id = ip.installment_id
        JOIN (
            
            
            SELECT
                li2.installment_id,
                IFNULL(
                    SUM(
                        CASE 
                            WHEN t2.status = 'Confirmed' 
                                 AND t2.transaction_id != NEW.transaction_id 
                            THEN ip2.payment_amount 
                            ELSE 0 
                        END
                    ), 
                    0
                ) AS remaining_confirmed_payments
            FROM tbl_loan_installment li2
            LEFT JOIN tbl_installment_payment ip2 
                ON li2.installment_id = ip2.installment_id
            LEFT JOIN tbl_transaction t2 
                ON ip2.transaction_id = t2.transaction_id
            WHERE li2.installment_id IN (
                SELECT installment_id 
                FROM tbl_installment_payment 
                WHERE transaction_id = NEW.transaction_id
            )
            GROUP BY li2.installment_id
        ) calc 
            ON li.installment_id = calc.installment_id
        SET
            li.installment_status = CASE
                WHEN li.installment_status = 'Paid'
                    THEN IF(li.installment_due_date < CURDATE(), 'Past Due', 'Active')
                ELSE li.installment_status
            END,
            li.is_partially_paid = IF(calc.remaining_confirmed_payments > 0, 1, 0)
        WHERE ip.transaction_id = NEW.transaction_id;

        
        UPDATE tbl_loan l
        SET l.loan_status = 'Active'
        WHERE l.loan_id IN (
            SELECT DISTINCT li_inner.loan_id
            FROM tbl_loan_installment li_inner
            JOIN tbl_installment_payment ip_inner 
                ON li_inner.installment_id = ip_inner.installment_id
            WHERE ip_inner.transaction_id = NEW.transaction_id
        )
        AND l.loan_status = 'Paid';

    END IF;
END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `trg_before_transaction_insert` BEFORE INSERT ON `tbl_transaction` FOR EACH ROW begin
if not exists (
select 1 from tbl_admin where employee_id = new.recorded_by and is_active = 1
union
select 1 from tbl_staff where employee_id = new.recorded_by and is_active = 1
) then
signal sqlstate '45000'
set message_text = 'You are not authorize for inserting transaction records.';
end if;
end
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Stand-in structure for view `vw_total_amount_installment`
-- (See below for the actual view)
--
CREATE TABLE `vw_total_amount_installment` (
`installment_id` int(11)
,`loan_id` int(11)
,`client_id` int(11)
,`installment_status` enum('Active','Paid','Past Due')
,`installment_amount` decimal(12,2)
,`installment_due_date` date
,`penalty_added` decimal(12,2)
,`total_amount_to_pay` decimal(35,2)
);

-- --------------------------------------------------------

--
-- Structure for view `vw_total_amount_installment`
--
DROP TABLE IF EXISTS `vw_total_amount_installment`;

CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `vw_total_amount_installment`  AS SELECT `li`.`installment_id` AS `installment_id`, `li`.`loan_id` AS `loan_id`, `l`.`client_id` AS `client_id`, `li`.`installment_status` AS `installment_status`, `li`.`installment_amount` AS `installment_amount`, `li`.`installment_due_date` AS `installment_due_date`, `pda`.`penalty_added` AS `penalty_added`, `li`.`installment_amount`+ ifnull(`pda`.`penalty_added`,0) - ifnull(sum(case when `t`.`status` = 'Confirmed' then `ip`.`payment_amount` else 0 end),0) AS `total_amount_to_pay` FROM ((((`tbl_loan_installment` `li` left join `tbl_loan` `l` on(`li`.`loan_id` = `l`.`loan_id`)) left join `tbl_past_due_account` `pda` on(`li`.`installment_id` = `pda`.`installment_id`)) left join `tbl_installment_payment` `ip` on(`li`.`installment_id` = `ip`.`installment_id`)) left join `tbl_transaction` `t` on(`ip`.`transaction_id` = `t`.`transaction_id`)) WHERE `l`.`is_void` = 0 GROUP BY `li`.`installment_id`, `li`.`loan_id`, `l`.`client_id`, `li`.`installment_status`, `li`.`installment_amount`, `li`.`installment_due_date`, `pda`.`penalty_added` ;

--
-- Indexes for dumped tables
--

--
-- Indexes for table `tbl_admin`
--
ALTER TABLE `tbl_admin`
  ADD PRIMARY KEY (`admin_id`),
  ADD KEY `fk_admin_employee_id` (`employee_id`);

--
-- Indexes for table `tbl_business`
--
ALTER TABLE `tbl_business`
  ADD PRIMARY KEY (`business_id`),
  ADD KEY `fk_business_client_id` (`client_id`);

--
-- Indexes for table `tbl_client`
--
ALTER TABLE `tbl_client`
  ADD PRIMARY KEY (`client_id`),
  ADD KEY `fk_client_updated_by` (`updated_by`);

--
-- Indexes for table `tbl_collection_assignment`
--
ALTER TABLE `tbl_collection_assignment`
  ADD PRIMARY KEY (`assignment_id`),
  ADD KEY `fk_assignment_past_due_id` (`past_due_id`),
  ADD KEY `fk_collection_assigned_to` (`assigned_to`),
  ADD KEY `fk_collection_creted_by` (`created_by`),
  ADD KEY `fk_collection_updated_by` (`updated_by`);

--
-- Indexes for table `tbl_collector`
--
ALTER TABLE `tbl_collector`
  ADD PRIMARY KEY (`collector_id`),
  ADD KEY `fk_collector_employee_id` (`employee_id`);

--
-- Indexes for table `tbl_employee`
--
ALTER TABLE `tbl_employee`
  ADD PRIMARY KEY (`employee_id`),
  ADD UNIQUE KEY `email` (`email`),
  ADD KEY `fk_employee_updated_by` (`updated_by`);

--
-- Indexes for table `tbl_employee_credential`
--
ALTER TABLE `tbl_employee_credential`
  ADD PRIMARY KEY (`credential_id`),
  ADD UNIQUE KEY `username` (`username`),
  ADD KEY `credential_employee_id` (`employee_id`);

--
-- Indexes for table `tbl_follow_up`
--
ALTER TABLE `tbl_follow_up`
  ADD PRIMARY KEY (`follow_up_id`),
  ADD KEY `fk_follow_past_due_id` (`past_due_id`),
  ADD KEY `fk_follow_recorded_by` (`recorded_by`),
  ADD KEY `fk_follow_up_updated_by` (`updated_by`);

--
-- Indexes for table `tbl_installment_payment`
--
ALTER TABLE `tbl_installment_payment`
  ADD PRIMARY KEY (`payment_id`),
  ADD KEY `fk_payment_installment_id` (`installment_id`),
  ADD KEY `fk_payment_recorded_by` (`recorded_by`),
  ADD KEY `fk_payment_transaction_id` (`transaction_id`);

--
-- Indexes for table `tbl_loan`
--
ALTER TABLE `tbl_loan`
  ADD PRIMARY KEY (`loan_id`),
  ADD KEY `fk_loan_client_id` (`client_id`),
  ADD KEY `fk_loan_approved_by` (`approved_by`),
  ADD KEY `fk_loan_updated_by` (`updated_by`);

--
-- Indexes for table `tbl_loan_installment`
--
ALTER TABLE `tbl_loan_installment`
  ADD PRIMARY KEY (`installment_id`),
  ADD KEY `fk_installment_loan_id` (`loan_id`),
  ADD KEY `fk_installment_updated_by` (`updated_by`);

--
-- Indexes for table `tbl_past_due_account`
--
ALTER TABLE `tbl_past_due_account`
  ADD PRIMARY KEY (`past_due_id`),
  ADD KEY `fk_past_installment_id` (`installment_id`),
  ADD KEY `fk_past_due_updated_by` (`updated_by`),
  ADD KEY `fk_past_due_added_by` (`added_by`);

--
-- Indexes for table `tbl_promise`
--
ALTER TABLE `tbl_promise`
  ADD PRIMARY KEY (`promise_id`),
  ADD KEY `fk_promise_past_due_id` (`past_due_id`),
  ADD KEY `fk_promise_recorded_by` (`recorded_by`),
  ADD KEY `fk_promise_updated_by` (`updated_by`);

--
-- Indexes for table `tbl_staff`
--
ALTER TABLE `tbl_staff`
  ADD PRIMARY KEY (`staff_id`),
  ADD KEY `fk_staff_employee_id` (`employee_id`);

--
-- Indexes for table `tbl_transaction`
--
ALTER TABLE `tbl_transaction`
  ADD PRIMARY KEY (`transaction_id`),
  ADD KEY `fk_transaction_recorded_by` (`recorded_by`),
  ADD KEY `fk_transaction_client_id` (`client_id`),
  ADD KEY `fk_transaction_updated_by` (`updated_by`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `tbl_admin`
--
ALTER TABLE `tbl_admin`
  MODIFY `admin_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT for table `tbl_business`
--
ALTER TABLE `tbl_business`
  MODIFY `business_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=12;

--
-- AUTO_INCREMENT for table `tbl_client`
--
ALTER TABLE `tbl_client`
  MODIFY `client_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- AUTO_INCREMENT for table `tbl_collection_assignment`
--
ALTER TABLE `tbl_collection_assignment`
  MODIFY `assignment_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=20;

--
-- AUTO_INCREMENT for table `tbl_collector`
--
ALTER TABLE `tbl_collector`
  MODIFY `collector_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT for table `tbl_employee`
--
ALTER TABLE `tbl_employee`
  MODIFY `employee_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=13;

--
-- AUTO_INCREMENT for table `tbl_employee_credential`
--
ALTER TABLE `tbl_employee_credential`
  MODIFY `credential_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=10;

--
-- AUTO_INCREMENT for table `tbl_follow_up`
--
ALTER TABLE `tbl_follow_up`
  MODIFY `follow_up_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=19;

--
-- AUTO_INCREMENT for table `tbl_installment_payment`
--
ALTER TABLE `tbl_installment_payment`
  MODIFY `payment_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=72;

--
-- AUTO_INCREMENT for table `tbl_loan`
--
ALTER TABLE `tbl_loan`
  MODIFY `loan_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=28;

--
-- AUTO_INCREMENT for table `tbl_loan_installment`
--
ALTER TABLE `tbl_loan_installment`
  MODIFY `installment_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=164;

--
-- AUTO_INCREMENT for table `tbl_past_due_account`
--
ALTER TABLE `tbl_past_due_account`
  MODIFY `past_due_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=37;

--
-- AUTO_INCREMENT for table `tbl_promise`
--
ALTER TABLE `tbl_promise`
  MODIFY `promise_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=31;

--
-- AUTO_INCREMENT for table `tbl_staff`
--
ALTER TABLE `tbl_staff`
  MODIFY `staff_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT for table `tbl_transaction`
--
ALTER TABLE `tbl_transaction`
  MODIFY `transaction_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=60;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `tbl_admin`
--
ALTER TABLE `tbl_admin`
  ADD CONSTRAINT `fk_admin_employee_id` FOREIGN KEY (`employee_id`) REFERENCES `tbl_employee` (`employee_id`);

--
-- Constraints for table `tbl_business`
--
ALTER TABLE `tbl_business`
  ADD CONSTRAINT `fk_business_client_id` FOREIGN KEY (`client_id`) REFERENCES `tbl_client` (`client_id`);

--
-- Constraints for table `tbl_client`
--
ALTER TABLE `tbl_client`
  ADD CONSTRAINT `fk_client_updated_by` FOREIGN KEY (`updated_by`) REFERENCES `tbl_employee` (`employee_id`);

--
-- Constraints for table `tbl_collection_assignment`
--
ALTER TABLE `tbl_collection_assignment`
  ADD CONSTRAINT `fk_assignment_past_due_id` FOREIGN KEY (`past_due_id`) REFERENCES `tbl_past_due_account` (`past_due_id`),
  ADD CONSTRAINT `fk_collection_assigned_to` FOREIGN KEY (`assigned_to`) REFERENCES `tbl_employee` (`employee_id`),
  ADD CONSTRAINT `fk_collection_creted_by` FOREIGN KEY (`created_by`) REFERENCES `tbl_employee` (`employee_id`),
  ADD CONSTRAINT `fk_collection_updated_by` FOREIGN KEY (`updated_by`) REFERENCES `tbl_employee` (`employee_id`);

--
-- Constraints for table `tbl_collector`
--
ALTER TABLE `tbl_collector`
  ADD CONSTRAINT `fk_collector_employee_id` FOREIGN KEY (`employee_id`) REFERENCES `tbl_employee` (`employee_id`);

--
-- Constraints for table `tbl_employee`
--
ALTER TABLE `tbl_employee`
  ADD CONSTRAINT `fk_employee_updated_by` FOREIGN KEY (`updated_by`) REFERENCES `tbl_employee` (`employee_id`);

--
-- Constraints for table `tbl_employee_credential`
--
ALTER TABLE `tbl_employee_credential`
  ADD CONSTRAINT `credential_employee_id` FOREIGN KEY (`employee_id`) REFERENCES `tbl_employee` (`employee_id`);

--
-- Constraints for table `tbl_follow_up`
--
ALTER TABLE `tbl_follow_up`
  ADD CONSTRAINT `fk_follow_past_due_id` FOREIGN KEY (`past_due_id`) REFERENCES `tbl_past_due_account` (`past_due_id`),
  ADD CONSTRAINT `fk_follow_recorded_by` FOREIGN KEY (`recorded_by`) REFERENCES `tbl_employee` (`employee_id`),
  ADD CONSTRAINT `fk_follow_up_updated_by` FOREIGN KEY (`updated_by`) REFERENCES `tbl_employee` (`employee_id`);

--
-- Constraints for table `tbl_installment_payment`
--
ALTER TABLE `tbl_installment_payment`
  ADD CONSTRAINT `fk_payment_installment_id` FOREIGN KEY (`installment_id`) REFERENCES `tbl_loan_installment` (`installment_id`),
  ADD CONSTRAINT `fk_payment_recorded_by` FOREIGN KEY (`recorded_by`) REFERENCES `tbl_employee` (`employee_id`),
  ADD CONSTRAINT `fk_payment_transaction_id` FOREIGN KEY (`transaction_id`) REFERENCES `tbl_transaction` (`transaction_id`);

--
-- Constraints for table `tbl_loan`
--
ALTER TABLE `tbl_loan`
  ADD CONSTRAINT `fk_loan_approved_by` FOREIGN KEY (`approved_by`) REFERENCES `tbl_employee` (`employee_id`),
  ADD CONSTRAINT `fk_loan_client_id` FOREIGN KEY (`client_id`) REFERENCES `tbl_client` (`client_id`),
  ADD CONSTRAINT `fk_loan_updated_by` FOREIGN KEY (`updated_by`) REFERENCES `tbl_employee` (`employee_id`);

--
-- Constraints for table `tbl_loan_installment`
--
ALTER TABLE `tbl_loan_installment`
  ADD CONSTRAINT `fk_installment_loan_id` FOREIGN KEY (`loan_id`) REFERENCES `tbl_loan` (`loan_id`),
  ADD CONSTRAINT `fk_installment_updated_by` FOREIGN KEY (`updated_by`) REFERENCES `tbl_employee` (`employee_id`);

--
-- Constraints for table `tbl_past_due_account`
--
ALTER TABLE `tbl_past_due_account`
  ADD CONSTRAINT `fk_past_due_added_by` FOREIGN KEY (`added_by`) REFERENCES `tbl_employee` (`employee_id`),
  ADD CONSTRAINT `fk_past_due_updated_by` FOREIGN KEY (`updated_by`) REFERENCES `tbl_employee` (`employee_id`),
  ADD CONSTRAINT `fk_past_installment_id` FOREIGN KEY (`installment_id`) REFERENCES `tbl_loan_installment` (`installment_id`);

--
-- Constraints for table `tbl_promise`
--
ALTER TABLE `tbl_promise`
  ADD CONSTRAINT `fk_promise_past_due_id` FOREIGN KEY (`past_due_id`) REFERENCES `tbl_past_due_account` (`past_due_id`),
  ADD CONSTRAINT `fk_promise_recorded_by` FOREIGN KEY (`recorded_by`) REFERENCES `tbl_employee` (`employee_id`),
  ADD CONSTRAINT `fk_promise_updated_by` FOREIGN KEY (`updated_by`) REFERENCES `tbl_employee` (`employee_id`);

--
-- Constraints for table `tbl_staff`
--
ALTER TABLE `tbl_staff`
  ADD CONSTRAINT `fk_staff_employee_id` FOREIGN KEY (`employee_id`) REFERENCES `tbl_employee` (`employee_id`);

--
-- Constraints for table `tbl_transaction`
--
ALTER TABLE `tbl_transaction`
  ADD CONSTRAINT `fk_transaction_client_id` FOREIGN KEY (`client_id`) REFERENCES `tbl_client` (`client_id`),
  ADD CONSTRAINT `fk_transaction_recorded_by` FOREIGN KEY (`recorded_by`) REFERENCES `tbl_employee` (`employee_id`),
  ADD CONSTRAINT `fk_transaction_updated_by` FOREIGN KEY (`updated_by`) REFERENCES `tbl_employee` (`employee_id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
