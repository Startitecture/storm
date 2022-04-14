--
-- PostgreSQL database dump
--

-- Dumped from database version 10.17
-- Dumped by pg_dump version 14.2

-- Started on 2022-04-01 12:39:01

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

DROP DATABASE "OrmTestModel";
--
-- TOC entry 4497 (class 1262 OID 21078)
-- Name: OrmTestModel; Type: DATABASE; Schema: -; Owner: -
--

CREATE DATABASE "OrmTestModel" WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE = 'English_United States.1252';


\connect "OrmTestModel"

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 10 (class 2615 OID 21082)
-- Name: dbo; Type: SCHEMA; Schema: -; Owner: -
--

CREATE SCHEMA dbo;


SET default_tablespace = '';

--
-- TOC entry 225 (class 1259 OID 21758)
-- Name: AggregateEventCompletion; Type: TABLE; Schema: dbo; Owner: -
--

CREATE TABLE dbo."AggregateEventCompletion" (
    "AggregateEventCompletionId" bigint NOT NULL,
    "EndTime" timestamp with time zone NOT NULL,
    "Succeeded" boolean NOT NULL,
    "Result" text NOT NULL
);


--
-- TOC entry 224 (class 1259 OID 21729)
-- Name: AggregateEventStart; Type: TABLE; Schema: dbo; Owner: -
--

CREATE TABLE dbo."AggregateEventStart" (
    "AggregateEventStartId" bigint NOT NULL,
    "DomainAggregateId" integer NOT NULL,
    "GlobalIdentifier" character(16) NOT NULL,
    "DomainIdentityId" integer NOT NULL,
    "StartTime" timestamp with time zone NOT NULL,
    "EventName" character varying(50) NOT NULL,
    "EventDescription" text NOT NULL
);


--
-- TOC entry 223 (class 1259 OID 21727)
-- Name: AggregateEventStartId_AggregateEventStartId_seq; Type: SEQUENCE; Schema: dbo; Owner: -
--

CREATE SEQUENCE dbo."AggregateEventStartId_AggregateEventStartId_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 4498 (class 0 OID 0)
-- Dependencies: 223
-- Name: AggregateEventStartId_AggregateEventStartId_seq; Type: SEQUENCE OWNED BY; Schema: dbo; Owner: -
--

ALTER SEQUENCE dbo."AggregateEventStartId_AggregateEventStartId_seq" OWNED BY dbo."AggregateEventStart"."AggregateEventStartId";


--
-- TOC entry 219 (class 1259 OID 21487)
-- Name: AggregateOption; Type: TABLE; Schema: dbo; Owner: -
--

CREATE TABLE dbo."AggregateOption" (
    "AggregateOptionId" integer NOT NULL,
    "Name" character varying(50) NOT NULL,
    "AggregateOptionTypeId" integer NOT NULL,
    "Value" numeric(38,4) NOT NULL
);


--
-- TOC entry 205 (class 1259 OID 21128)
-- Name: AggregateOptionType; Type: TABLE; Schema: dbo; Owner: -
--

CREATE TABLE dbo."AggregateOptionType" (
    "AggregateOptionTypeId" integer NOT NULL,
    "Name" character varying(50) NOT NULL
);


--
-- TOC entry 233 (class 1259 OID 22282)
-- Name: AggregateSubmission; Type: TABLE; Schema: dbo; Owner: -
--

CREATE TABLE dbo."AggregateSubmission" (
    "AggregateSubmissionId" integer NOT NULL,
    "DomainAggregateId" integer NOT NULL
);


--
-- TOC entry 222 (class 1259 OID 21566)
-- Name: Association; Type: TABLE; Schema: dbo; Owner: -
--

CREATE TABLE dbo."Association" (
    "OtherAggregateId" integer NOT NULL,
    "DomainAggregateId" integer NOT NULL
);


--
-- TOC entry 207 (class 1259 OID 21139)
-- Name: CategoryAttribute; Type: TABLE; Schema: dbo; Owner: -
--

CREATE TABLE dbo."CategoryAttribute" (
    "CategoryAttributeId" integer NOT NULL,
    "Name" character varying(50) NOT NULL,
    "IsSystem" boolean NOT NULL,
    "IsActive" boolean NOT NULL
);


--
-- TOC entry 206 (class 1259 OID 21137)
-- Name: CategoryAttribute_CategoryAttributeId_seq; Type: SEQUENCE; Schema: dbo; Owner: -
--

CREATE SEQUENCE dbo."CategoryAttribute_CategoryAttributeId_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 4499 (class 0 OID 0)
-- Dependencies: 206
-- Name: CategoryAttribute_CategoryAttributeId_seq; Type: SEQUENCE OWNED BY; Schema: dbo; Owner: -
--

ALTER SEQUENCE dbo."CategoryAttribute_CategoryAttributeId_seq" OWNED BY dbo."CategoryAttribute"."CategoryAttributeId";


--
-- TOC entry 221 (class 1259 OID 21540)
-- Name: Child; Type: TABLE; Schema: dbo; Owner: -
--

CREATE TABLE dbo."Child" (
    "ChildId" integer NOT NULL,
    "DomainAggregateId" integer NOT NULL,
    "Name" character varying(50) NOT NULL,
    "Value" money NOT NULL
);


--
-- TOC entry 220 (class 1259 OID 21538)
-- Name: Child_ChildId_seq; Type: SEQUENCE; Schema: dbo; Owner: -
--

CREATE SEQUENCE dbo."Child_ChildId_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 4500 (class 0 OID 0)
-- Dependencies: 220
-- Name: Child_ChildId_seq; Type: SEQUENCE OWNED BY; Schema: dbo; Owner: -
--

ALTER SEQUENCE dbo."Child_ChildId_seq" OWNED BY dbo."Child"."ChildId";


--
-- TOC entry 234 (class 1259 OID 22312)
-- Name: CurrentAggregateSubmission; Type: TABLE; Schema: dbo; Owner: -
--

CREATE TABLE dbo."CurrentAggregateSubmission" (
    "CurrentAggregateSubmissionId" integer NOT NULL,
    "DomainAggregateId" integer NOT NULL
);


--
-- TOC entry 236 (class 1259 OID 22353)
-- Name: DateElement; Type: TABLE; Schema: dbo; Owner: -
--

CREATE TABLE dbo."DateElement" (
    "DateElementId" bigint NOT NULL,
    "Value" timestamp with time zone NOT NULL
);


--
-- TOC entry 218 (class 1259 OID 21450)
-- Name: DomainAggregate; Type: TABLE; Schema: dbo; Owner: -
--

CREATE TABLE dbo."DomainAggregate" (
    "DomainAggregateId" integer NOT NULL,
    "SubContainerId" integer NOT NULL,
    "TemplateId" integer NOT NULL,
    "CategoryAttributeId" integer NOT NULL,
    "Name" character varying(50) NOT NULL,
    "Description" text NOT NULL,
    "CreatedByDomainIdentityId" integer NOT NULL,
    "CreatedTime" timestamp with time zone NOT NULL,
    "LastModifiedByDomainIdentityId" integer NOT NULL,
    "LastModifiedTime" timestamp with time zone NOT NULL
);


--
-- TOC entry 235 (class 1259 OID 22332)
-- Name: DomainAggregateFlagAttribute; Type: TABLE; Schema: dbo; Owner: -
--

CREATE TABLE dbo."DomainAggregateFlagAttribute" (
    "DomainAggregateId" integer NOT NULL,
    "FlagAttributeId" integer NOT NULL
);


--
-- TOC entry 217 (class 1259 OID 21448)
-- Name: DomainAggregate_DomainAggregateId_seq; Type: SEQUENCE; Schema: dbo; Owner: -
--

CREATE SEQUENCE dbo."DomainAggregate_DomainAggregateId_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 4501 (class 0 OID 0)
-- Dependencies: 217
-- Name: DomainAggregate_DomainAggregateId_seq; Type: SEQUENCE OWNED BY; Schema: dbo; Owner: -
--

ALTER SEQUENCE dbo."DomainAggregate_DomainAggregateId_seq" OWNED BY dbo."DomainAggregate"."DomainAggregateId";


--
-- TOC entry 209 (class 1259 OID 21150)
-- Name: DomainIdentity; Type: TABLE; Schema: dbo; Owner: -
--

CREATE TABLE dbo."DomainIdentity" (
    "DomainIdentityId" integer NOT NULL,
    "UniqueIdentifier" character varying(254) NOT NULL,
    "FirstName" character varying(50) NOT NULL,
    "MiddleName" character varying(50),
    "LastName" character varying(50) NOT NULL
);


--
-- TOC entry 208 (class 1259 OID 21148)
-- Name: DomainIdentity_DomainIdentityId_seq; Type: SEQUENCE; Schema: dbo; Owner: -
--

CREATE SEQUENCE dbo."DomainIdentity_DomainIdentityId_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 4502 (class 0 OID 0)
-- Dependencies: 208
-- Name: DomainIdentity_DomainIdentityId_seq; Type: SEQUENCE OWNED BY; Schema: dbo; Owner: -
--

ALTER SEQUENCE dbo."DomainIdentity_DomainIdentityId_seq" OWNED BY dbo."DomainIdentity"."DomainIdentityId";


--
-- TOC entry 211 (class 1259 OID 21172)
-- Name: Field; Type: TABLE; Schema: dbo; Owner: -
--

CREATE TABLE dbo."Field" (
    "FieldId" integer NOT NULL,
    "Name" character varying(50) NOT NULL,
    "Description" text NOT NULL
);


--
-- TOC entry 227 (class 1259 OID 21776)
-- Name: FieldValue; Type: TABLE; Schema: dbo; Owner: -
--

CREATE TABLE dbo."FieldValue" (
    "FieldValueId" bigint NOT NULL,
    "FieldId" integer NOT NULL,
    "LastModifiedByDomainIdentifierId" integer NOT NULL,
    "LastModifiedTime" timestamp with time zone NOT NULL
);


--
-- TOC entry 229 (class 1259 OID 22205)
-- Name: FieldValueElement; Type: TABLE; Schema: dbo; Owner: -
--

CREATE TABLE dbo."FieldValueElement" (
    "FieldValueElementId" bigint NOT NULL,
    "FieldValueId" bigint NOT NULL,
    "Order" integer NOT NULL
);


--
-- TOC entry 228 (class 1259 OID 22203)
-- Name: FieldValueElement_FieldValueElementId_seq; Type: SEQUENCE; Schema: dbo; Owner: -
--

CREATE SEQUENCE dbo."FieldValueElement_FieldValueElementId_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 4503 (class 0 OID 0)
-- Dependencies: 228
-- Name: FieldValueElement_FieldValueElementId_seq; Type: SEQUENCE OWNED BY; Schema: dbo; Owner: -
--

ALTER SEQUENCE dbo."FieldValueElement_FieldValueElementId_seq" OWNED BY dbo."FieldValueElement"."FieldValueElementId";


--
-- TOC entry 226 (class 1259 OID 21774)
-- Name: FieldValue_FieldValueId_seq; Type: SEQUENCE; Schema: dbo; Owner: -
--

CREATE SEQUENCE dbo."FieldValue_FieldValueId_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 4504 (class 0 OID 0)
-- Dependencies: 226
-- Name: FieldValue_FieldValueId_seq; Type: SEQUENCE OWNED BY; Schema: dbo; Owner: -
--

ALTER SEQUENCE dbo."FieldValue_FieldValueId_seq" OWNED BY dbo."FieldValue"."FieldValueId";


--
-- TOC entry 210 (class 1259 OID 21170)
-- Name: Field_FieldId_seq; Type: SEQUENCE; Schema: dbo; Owner: -
--

CREATE SEQUENCE dbo."Field_FieldId_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 4505 (class 0 OID 0)
-- Dependencies: 210
-- Name: Field_FieldId_seq; Type: SEQUENCE OWNED BY; Schema: dbo; Owner: -
--

ALTER SEQUENCE dbo."Field_FieldId_seq" OWNED BY dbo."Field"."FieldId";


--
-- TOC entry 241 (class 1259 OID 83874)
-- Name: FlagAttribute_FlagAttributeId_seq; Type: SEQUENCE; Schema: dbo; Owner: -
--

CREATE SEQUENCE dbo."FlagAttribute_FlagAttributeId_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 212 (class 1259 OID 21185)
-- Name: FlagAttribute; Type: TABLE; Schema: dbo; Owner: -
--

CREATE TABLE dbo."FlagAttribute" (
    "FlagAttributeId" integer DEFAULT nextval('dbo."FlagAttribute_FlagAttributeId_seq"'::regclass) NOT NULL,
    "Name" character varying(50) NOT NULL
);


--
-- TOC entry 237 (class 1259 OID 22366)
-- Name: FloatElement; Type: TABLE; Schema: dbo; Owner: -
--

CREATE TABLE dbo."FloatElement" (
    "FloatElementId" bigint NOT NULL,
    "Value" double precision NOT NULL
);


--
-- TOC entry 231 (class 1259 OID 22235)
-- Name: GenericSubmission; Type: TABLE; Schema: dbo; Owner: -
--

CREATE TABLE dbo."GenericSubmission" (
    "GenericSubmissionId" integer NOT NULL,
    "Subject" character varying(50) NOT NULL,
    "SubmittedByDomainIdentifierId" integer NOT NULL,
    "SubmittedTime" timestamp with time zone NOT NULL
);


--
-- TOC entry 232 (class 1259 OID 22249)
-- Name: GenericSubmissionValue; Type: TABLE; Schema: dbo; Owner: -
--

CREATE TABLE dbo."GenericSubmissionValue" (
    "GenericSubmissionValueId" bigint NOT NULL,
    "GenericSubmissionId" integer NOT NULL
);


--
-- TOC entry 230 (class 1259 OID 22233)
-- Name: GenericSubmission_GenericSubmissionId_seq; Type: SEQUENCE; Schema: dbo; Owner: -
--

CREATE SEQUENCE dbo."GenericSubmission_GenericSubmissionId_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 4506 (class 0 OID 0)
-- Dependencies: 230
-- Name: GenericSubmission_GenericSubmissionId_seq; Type: SEQUENCE OWNED BY; Schema: dbo; Owner: -
--

ALTER SEQUENCE dbo."GenericSubmission_GenericSubmissionId_seq" OWNED BY dbo."GenericSubmission"."GenericSubmissionId";


--
-- TOC entry 238 (class 1259 OID 22381)
-- Name: IntegerElement; Type: TABLE; Schema: dbo; Owner: -
--

CREATE TABLE dbo."IntegerElement" (
    "IntegerElementId" bigint NOT NULL,
    "Value" bigint NOT NULL
);


--
-- TOC entry 239 (class 1259 OID 22394)
-- Name: MoneyElement; Type: TABLE; Schema: dbo; Owner: -
--

CREATE TABLE dbo."MoneyElement" (
    "MoneyElementId" bigint NOT NULL,
    "Value" money NOT NULL
);


--
-- TOC entry 216 (class 1259 OID 21422)
-- Name: OtherAggregate; Type: TABLE; Schema: dbo; Owner: -
--

CREATE TABLE dbo."OtherAggregate" (
    "OtherAggregateId" integer NOT NULL,
    "Name" character varying(50) NOT NULL,
    "AggregateOptionTypeId" integer NOT NULL
);


--
-- TOC entry 215 (class 1259 OID 21420)
-- Name: OtherAggregate_OtherAggregateId_seq; Type: SEQUENCE; Schema: dbo; Owner: -
--

CREATE SEQUENCE dbo."OtherAggregate_OtherAggregateId_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 4507 (class 0 OID 0)
-- Dependencies: 215
-- Name: OtherAggregate_OtherAggregateId_seq; Type: SEQUENCE OWNED BY; Schema: dbo; Owner: -
--

ALTER SEQUENCE dbo."OtherAggregate_OtherAggregateId_seq" OWNED BY dbo."OtherAggregate"."OtherAggregateId";


--
-- TOC entry 214 (class 1259 OID 21271)
-- Name: SubContainer; Type: TABLE; Schema: dbo; Owner: -
--

CREATE TABLE dbo."SubContainer" (
    "SubContainerId" integer NOT NULL,
    "TopContainerId" integer NOT NULL,
    "Name" character varying(50) NOT NULL
);


--
-- TOC entry 213 (class 1259 OID 21269)
-- Name: SubContainer_SubContainerId_seq; Type: SEQUENCE; Schema: dbo; Owner: -
--

CREATE SEQUENCE dbo."SubContainer_SubContainerId_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 4508 (class 0 OID 0)
-- Dependencies: 213
-- Name: SubContainer_SubContainerId_seq; Type: SEQUENCE OWNED BY; Schema: dbo; Owner: -
--

ALTER SEQUENCE dbo."SubContainer_SubContainerId_seq" OWNED BY dbo."SubContainer"."SubContainerId";


--
-- TOC entry 204 (class 1259 OID 21111)
-- Name: Template; Type: TABLE; Schema: dbo; Owner: -
--

CREATE TABLE dbo."Template" (
    "TemplateId" integer NOT NULL,
    "Name" character varying(50) NOT NULL
);


--
-- TOC entry 203 (class 1259 OID 21109)
-- Name: Template_TemplateId_seq; Type: SEQUENCE; Schema: dbo; Owner: -
--

CREATE SEQUENCE dbo."Template_TemplateId_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 4509 (class 0 OID 0)
-- Dependencies: 203
-- Name: Template_TemplateId_seq; Type: SEQUENCE OWNED BY; Schema: dbo; Owner: -
--

ALTER SEQUENCE dbo."Template_TemplateId_seq" OWNED BY dbo."Template"."TemplateId";


--
-- TOC entry 240 (class 1259 OID 22404)
-- Name: TextElement; Type: TABLE; Schema: dbo; Owner: -
--

CREATE TABLE dbo."TextElement" (
    "TextElementId" bigint NOT NULL,
    "Value" text NOT NULL
);


--
-- TOC entry 202 (class 1259 OID 21100)
-- Name: TopContainer; Type: TABLE; Schema: dbo; Owner: -
--

CREATE TABLE dbo."TopContainer" (
    "TopContainerId" integer NOT NULL,
    "Name" character varying(50) NOT NULL
);


--
-- TOC entry 201 (class 1259 OID 21098)
-- Name: TopContainer_TopContainerId_seq; Type: SEQUENCE; Schema: dbo; Owner: -
--

CREATE SEQUENCE dbo."TopContainer_TopContainerId_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 4510 (class 0 OID 0)
-- Dependencies: 201
-- Name: TopContainer_TopContainerId_seq; Type: SEQUENCE OWNED BY; Schema: dbo; Owner: -
--

ALTER SEQUENCE dbo."TopContainer_TopContainerId_seq" OWNED BY dbo."TopContainer"."TopContainerId";


--
-- TOC entry 4230 (class 2604 OID 21732)
-- Name: AggregateEventStart AggregateEventStartId; Type: DEFAULT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."AggregateEventStart" ALTER COLUMN "AggregateEventStartId" SET DEFAULT nextval('dbo."AggregateEventStartId_AggregateEventStartId_seq"'::regclass);


--
-- TOC entry 4222 (class 2604 OID 21142)
-- Name: CategoryAttribute CategoryAttributeId; Type: DEFAULT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."CategoryAttribute" ALTER COLUMN "CategoryAttributeId" SET DEFAULT nextval('dbo."CategoryAttribute_CategoryAttributeId_seq"'::regclass);


--
-- TOC entry 4229 (class 2604 OID 21543)
-- Name: Child ChildId; Type: DEFAULT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."Child" ALTER COLUMN "ChildId" SET DEFAULT nextval('dbo."Child_ChildId_seq"'::regclass);


--
-- TOC entry 4228 (class 2604 OID 21453)
-- Name: DomainAggregate DomainAggregateId; Type: DEFAULT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."DomainAggregate" ALTER COLUMN "DomainAggregateId" SET DEFAULT nextval('dbo."DomainAggregate_DomainAggregateId_seq"'::regclass);


--
-- TOC entry 4223 (class 2604 OID 21153)
-- Name: DomainIdentity DomainIdentityId; Type: DEFAULT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."DomainIdentity" ALTER COLUMN "DomainIdentityId" SET DEFAULT nextval('dbo."DomainIdentity_DomainIdentityId_seq"'::regclass);


--
-- TOC entry 4224 (class 2604 OID 21175)
-- Name: Field FieldId; Type: DEFAULT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."Field" ALTER COLUMN "FieldId" SET DEFAULT nextval('dbo."Field_FieldId_seq"'::regclass);


--
-- TOC entry 4231 (class 2604 OID 21779)
-- Name: FieldValue FieldValueId; Type: DEFAULT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."FieldValue" ALTER COLUMN "FieldValueId" SET DEFAULT nextval('dbo."FieldValue_FieldValueId_seq"'::regclass);


--
-- TOC entry 4232 (class 2604 OID 22208)
-- Name: FieldValueElement FieldValueElementId; Type: DEFAULT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."FieldValueElement" ALTER COLUMN "FieldValueElementId" SET DEFAULT nextval('dbo."FieldValueElement_FieldValueElementId_seq"'::regclass);


--
-- TOC entry 4233 (class 2604 OID 22238)
-- Name: GenericSubmission GenericSubmissionId; Type: DEFAULT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."GenericSubmission" ALTER COLUMN "GenericSubmissionId" SET DEFAULT nextval('dbo."GenericSubmission_GenericSubmissionId_seq"'::regclass);


--
-- TOC entry 4227 (class 2604 OID 21425)
-- Name: OtherAggregate OtherAggregateId; Type: DEFAULT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."OtherAggregate" ALTER COLUMN "OtherAggregateId" SET DEFAULT nextval('dbo."OtherAggregate_OtherAggregateId_seq"'::regclass);


--
-- TOC entry 4226 (class 2604 OID 21274)
-- Name: SubContainer SubContainerId; Type: DEFAULT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."SubContainer" ALTER COLUMN "SubContainerId" SET DEFAULT nextval('dbo."SubContainer_SubContainerId_seq"'::regclass);


--
-- TOC entry 4221 (class 2604 OID 21114)
-- Name: Template TemplateId; Type: DEFAULT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."Template" ALTER COLUMN "TemplateId" SET DEFAULT nextval('dbo."Template_TemplateId_seq"'::regclass);


--
-- TOC entry 4220 (class 2604 OID 21103)
-- Name: TopContainer TopContainerId; Type: DEFAULT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."TopContainer" ALTER COLUMN "TopContainerId" SET DEFAULT nextval('dbo."TopContainer_TopContainerId_seq"'::regclass);


--
-- TOC entry 4475 (class 0 OID 21758)
-- Dependencies: 225
-- Data for Name: AggregateEventCompletion; Type: TABLE DATA; Schema: dbo; Owner: -
--

COPY dbo."AggregateEventCompletion" ("AggregateEventCompletionId", "EndTime", "Succeeded", "Result") FROM stdin;
\.


--
-- TOC entry 4474 (class 0 OID 21729)
-- Dependencies: 224
-- Data for Name: AggregateEventStart; Type: TABLE DATA; Schema: dbo; Owner: -
--

COPY dbo."AggregateEventStart" ("AggregateEventStartId", "DomainAggregateId", "GlobalIdentifier", "DomainIdentityId", "StartTime", "EventName", "EventDescription") FROM stdin;
\.


--
-- TOC entry 4469 (class 0 OID 21487)
-- Dependencies: 219
-- Data for Name: AggregateOption; Type: TABLE DATA; Schema: dbo; Owner: -
--

COPY dbo."AggregateOption" ("AggregateOptionId", "Name", "AggregateOptionTypeId", "Value") FROM stdin;
\.


--
-- TOC entry 4455 (class 0 OID 21128)
-- Dependencies: 205
-- Data for Name: AggregateOptionType; Type: TABLE DATA; Schema: dbo; Owner: -
--

COPY dbo."AggregateOptionType" ("AggregateOptionTypeId", "Name") FROM stdin;
1	OptionA
2	OptionB
3	OptionC
4	OptionD
5	OptionE
\.


--
-- TOC entry 4483 (class 0 OID 22282)
-- Dependencies: 233
-- Data for Name: AggregateSubmission; Type: TABLE DATA; Schema: dbo; Owner: -
--

COPY dbo."AggregateSubmission" ("AggregateSubmissionId", "DomainAggregateId") FROM stdin;
\.


--
-- TOC entry 4472 (class 0 OID 21566)
-- Dependencies: 222
-- Data for Name: Association; Type: TABLE DATA; Schema: dbo; Owner: -
--

COPY dbo."Association" ("OtherAggregateId", "DomainAggregateId") FROM stdin;
\.


--
-- TOC entry 4457 (class 0 OID 21139)
-- Dependencies: 207
-- Data for Name: CategoryAttribute; Type: TABLE DATA; Schema: dbo; Owner: -
--

COPY dbo."CategoryAttribute" ("CategoryAttributeId", "Name", "IsSystem", "IsActive") FROM stdin;
\.


--
-- TOC entry 4471 (class 0 OID 21540)
-- Dependencies: 221
-- Data for Name: Child; Type: TABLE DATA; Schema: dbo; Owner: -
--

COPY dbo."Child" ("ChildId", "DomainAggregateId", "Name", "Value") FROM stdin;
\.


--
-- TOC entry 4484 (class 0 OID 22312)
-- Dependencies: 234
-- Data for Name: CurrentAggregateSubmission; Type: TABLE DATA; Schema: dbo; Owner: -
--

COPY dbo."CurrentAggregateSubmission" ("CurrentAggregateSubmissionId", "DomainAggregateId") FROM stdin;
\.


--
-- TOC entry 4486 (class 0 OID 22353)
-- Dependencies: 236
-- Data for Name: DateElement; Type: TABLE DATA; Schema: dbo; Owner: -
--

COPY dbo."DateElement" ("DateElementId", "Value") FROM stdin;
42	2020-08-17 00:00:00+00
61	2020-08-17 00:00:00+00
80	2020-08-20 00:00:00+00
90	2020-08-20 00:00:00+00
109	2020-08-21 00:00:00+00
118	2020-08-21 02:12:15.910138+00
128	2020-08-21 00:00:00+00
147	2020-08-25 00:00:00+00
156	2020-08-25 18:43:23.217669+00
166	2020-08-25 00:00:00+00
185	2020-08-26 00:00:00+00
194	2020-08-26 00:45:50.445783+00
204	2020-08-26 00:00:00+00
223	2020-08-28 00:00:00+00
232	2020-08-28 14:57:42.618152+00
242	2020-08-28 00:00:00+00
261	2020-09-17 00:00:00+00
270	2020-09-17 02:06:07.480838+00
280	2020-09-17 00:00:00+00
299	2020-10-01 00:00:00+00
308	2020-10-01 16:38:23.546087+00
318	2020-10-01 00:00:00+00
337	2020-10-11 00:00:00+00
346	2020-10-11 00:42:56.165772+00
356	2020-10-11 00:00:00+00
375	2020-10-15 00:00:00+00
384	2020-10-15 20:51:44.228516+00
394	2020-10-15 00:00:00+00
403	2020-10-15 20:51:44.942497+00
413	2020-10-15 00:00:00+00
432	2020-10-15 00:00:00+00
441	2020-10-15 23:43:43.484507+00
451	2020-10-15 00:00:00+00
460	2020-10-15 23:43:43.950218+00
470	2020-10-15 00:00:00+00
489	2020-10-16 00:00:00+00
498	2020-10-16 18:37:08.701848+00
508	2020-10-16 00:00:00+00
517	2020-10-16 18:37:09.177467+00
527	2020-10-16 00:00:00+00
546	2020-10-16 00:00:00+00
555	2020-10-16 20:47:42.228993+00
565	2020-10-16 00:00:00+00
574	2020-10-16 20:47:42.75857+00
584	2020-10-16 00:00:00+00
603	2020-10-17 00:00:00+00
612	2020-10-17 02:05:26.345484+00
622	2020-10-17 00:00:00+00
631	2020-10-17 02:05:27.075661+00
641	2020-10-17 00:00:00+00
660	2020-11-03 00:00:00+00
669	2020-11-03 14:57:11.693899+00
679	2020-11-03 00:00:00+00
688	2020-11-03 14:57:12.16044+00
698	2020-11-03 00:00:00+00
717	2021-01-08 00:00:00+00
726	2021-01-08 05:26:24.844276+00
736	2021-01-08 00:00:00+00
745	2021-01-08 05:26:25.336793+00
755	2021-01-08 00:00:00+00
774	2021-01-08 00:00:00+00
783	2021-01-08 05:56:57.222506+00
793	2021-01-08 00:00:00+00
802	2021-01-08 05:56:57.718714+00
812	2021-01-08 00:00:00+00
831	2021-01-10 00:00:00+00
840	2021-01-10 19:21:37.636202+00
850	2021-01-10 00:00:00+00
859	2021-01-10 19:21:38.286675+00
869	2021-01-10 00:00:00+00
888	2021-01-10 00:00:00+00
897	2021-01-10 20:58:23.659898+00
907	2021-01-10 00:00:00+00
916	2021-01-10 20:58:24.13135+00
926	2021-01-10 00:00:00+00
945	2021-01-12 00:00:00+00
954	2021-01-12 16:45:48.066209+00
964	2021-01-12 00:00:00+00
973	2021-01-12 16:45:48.682462+00
983	2021-01-12 00:00:00+00
1002	2021-01-13 00:00:00+00
1011	2021-01-13 19:22:02.417345+00
1021	2021-01-13 00:00:00+00
1030	2021-01-13 19:22:03.029244+00
1040	2021-01-13 00:00:00+00
1059	2021-01-14 00:00:00+00
1068	2021-01-14 04:28:07.928592+00
1078	2021-01-14 00:00:00+00
1087	2021-01-14 04:28:08.672791+00
1097	2021-01-14 00:00:00+00
1116	2021-01-21 00:00:00+00
1125	2021-01-21 18:56:41.575067+00
1135	2021-01-21 00:00:00+00
1144	2021-01-21 18:56:45.070811+00
1154	2021-01-21 00:00:00+00
1173	2021-01-22 00:00:00+00
1182	2021-01-22 20:34:49.294334+00
1192	2021-01-22 00:00:00+00
1201	2021-01-22 20:34:50.217284+00
1211	2021-01-22 00:00:00+00
1230	2021-01-25 00:00:00+00
1239	2021-01-25 15:12:21.264005+00
1249	2021-01-25 00:00:00+00
1258	2021-01-25 15:12:21.695292+00
1268	2021-01-25 00:00:00+00
1287	2021-01-28 00:00:00+00
1296	2021-01-28 19:08:04.044712+00
1306	2021-01-28 00:00:00+00
1315	2021-01-28 19:08:07.392196+00
1325	2021-01-28 00:00:00+00
1344	2021-02-05 00:00:00+00
1353	2021-02-05 04:41:16.665961+00
1363	2021-02-05 00:00:00+00
1372	2021-02-05 04:41:20.152897+00
1382	2021-02-05 00:00:00+00
1401	2021-02-06 00:00:00+00
1410	2021-02-06 04:45:24.621199+00
1420	2021-02-06 00:00:00+00
1429	2021-02-06 04:45:28.294808+00
1439	2021-02-06 00:00:00+00
1458	2021-02-09 00:00:00+00
1467	2021-02-09 15:49:28.839727+00
1477	2021-02-09 00:00:00+00
1486	2021-02-09 15:49:32.583028+00
1496	2021-02-09 00:00:00+00
1515	2021-02-09 00:00:00+00
1524	2021-02-09 15:52:01.274835+00
1534	2021-02-09 00:00:00+00
1543	2021-02-09 15:52:04.99011+00
1553	2021-02-09 00:00:00+00
1572	2021-02-09 00:00:00+00
1581	2021-02-09 15:54:39.556237+00
1591	2021-02-09 00:00:00+00
1600	2021-02-09 15:54:43.330635+00
1610	2021-02-09 00:00:00+00
1629	2021-02-09 00:00:00+00
1638	2021-02-09 15:57:10.099865+00
1648	2021-02-09 00:00:00+00
1657	2021-02-09 15:57:13.807044+00
1667	2021-02-09 00:00:00+00
1686	2021-02-09 00:00:00+00
1695	2021-02-09 16:06:10.149702+00
1705	2021-02-09 00:00:00+00
1714	2021-02-09 16:06:11.634421+00
1724	2021-02-09 00:00:00+00
1743	2021-02-09 00:00:00+00
1752	2021-02-09 16:07:24.143722+00
1762	2021-02-09 00:00:00+00
1771	2021-02-09 16:07:25.576517+00
1781	2021-02-09 00:00:00+00
1800	2021-02-09 00:00:00+00
1809	2021-02-09 16:08:42.783361+00
1819	2021-02-09 00:00:00+00
1828	2021-02-09 16:08:44.239164+00
1838	2021-02-09 00:00:00+00
1857	2021-02-09 00:00:00+00
1866	2021-02-09 16:09:56.66588+00
1876	2021-02-09 00:00:00+00
1885	2021-02-09 16:09:58.134961+00
1895	2021-02-09 00:00:00+00
1914	2021-02-09 00:00:00+00
1923	2021-02-09 19:44:18.325122+00
1933	2021-02-09 00:00:00+00
1942	2021-02-09 19:44:19.600351+00
1952	2021-02-09 00:00:00+00
1971	2021-02-09 00:00:00+00
1980	2021-02-09 19:45:29.267477+00
1990	2021-02-09 00:00:00+00
1999	2021-02-09 19:45:30.626094+00
2009	2021-02-09 00:00:00+00
2028	2021-02-09 00:00:00+00
2037	2021-02-09 19:46:44.81494+00
2047	2021-02-09 00:00:00+00
2056	2021-02-09 19:46:46.137896+00
2066	2021-02-09 00:00:00+00
2085	2021-02-09 00:00:00+00
2094	2021-02-09 19:47:54.599822+00
2104	2021-02-09 00:00:00+00
2113	2021-02-09 19:47:55.841761+00
2123	2021-02-09 00:00:00+00
2142	2021-02-09 00:00:00+00
2151	2021-02-09 20:51:12.723934+00
2161	2021-02-09 00:00:00+00
2170	2021-02-09 20:51:14.054094+00
2180	2021-02-09 00:00:00+00
2199	2021-02-09 00:00:00+00
2208	2021-02-09 20:52:16.494461+00
2218	2021-02-09 00:00:00+00
2227	2021-02-09 20:52:17.789392+00
2237	2021-02-09 00:00:00+00
2256	2021-02-09 00:00:00+00
2265	2021-02-09 20:53:25.545981+00
2275	2021-02-09 00:00:00+00
2284	2021-02-09 20:53:26.851494+00
2294	2021-02-09 00:00:00+00
2313	2021-02-09 00:00:00+00
2322	2021-02-09 20:54:30.475573+00
2332	2021-02-09 00:00:00+00
2341	2021-02-09 20:54:31.77251+00
2351	2021-02-09 00:00:00+00
2370	2021-02-09 00:00:00+00
2379	2021-02-09 21:35:09.774017+00
2389	2021-02-09 00:00:00+00
2398	2021-02-09 21:35:10.178012+00
2408	2021-02-09 00:00:00+00
2427	2021-02-09 00:00:00+00
2436	2021-02-09 21:35:45.372094+00
2446	2021-02-09 00:00:00+00
2455	2021-02-09 21:35:45.76781+00
2465	2021-02-09 00:00:00+00
2484	2021-02-09 00:00:00+00
2493	2021-02-09 21:36:28.554374+00
2503	2021-02-09 00:00:00+00
2512	2021-02-09 21:36:28.990234+00
2522	2021-02-09 00:00:00+00
2541	2021-02-09 00:00:00+00
2550	2021-02-09 21:37:03.921249+00
2560	2021-02-09 00:00:00+00
2569	2021-02-09 21:37:04.292733+00
2579	2021-02-09 00:00:00+00
2598	2021-02-09 00:00:00+00
2607	2021-02-09 21:59:14.866487+00
2617	2021-02-09 00:00:00+00
2626	2021-02-09 21:59:15.455546+00
2636	2021-02-09 00:00:00+00
2655	2021-02-09 00:00:00+00
2664	2021-02-09 21:59:46.044992+00
2674	2021-02-09 00:00:00+00
2683	2021-02-09 21:59:46.66055+00
2693	2021-02-09 00:00:00+00
2712	2021-02-09 00:00:00+00
2721	2021-02-09 22:00:23.64341+00
2731	2021-02-09 00:00:00+00
2740	2021-02-09 22:00:24.249276+00
2750	2021-02-09 00:00:00+00
2769	2021-02-09 00:00:00+00
2778	2021-02-09 22:00:53.876906+00
2788	2021-02-09 00:00:00+00
2797	2021-02-09 22:00:54.419334+00
2807	2021-02-09 00:00:00+00
2826	2021-02-10 00:00:00+00
2835	2021-02-10 00:17:42.542429+00
2845	2021-02-10 00:00:00+00
2854	2021-02-10 00:17:45.865684+00
2864	2021-02-10 00:00:00+00
2883	2021-02-10 00:00:00+00
2892	2021-02-10 00:20:07.382308+00
2902	2021-02-10 00:00:00+00
2911	2021-02-10 00:20:10.707663+00
2921	2021-02-10 00:00:00+00
2940	2021-02-10 00:00:00+00
2949	2021-02-10 00:22:36.139304+00
2959	2021-02-10 00:00:00+00
2968	2021-02-10 00:22:39.579029+00
2978	2021-02-10 00:00:00+00
2997	2021-02-10 00:00:00+00
3006	2021-02-10 00:24:56.869273+00
3016	2021-02-10 00:00:00+00
3025	2021-02-10 00:25:00.255205+00
3035	2021-02-10 00:00:00+00
\.


--
-- TOC entry 4468 (class 0 OID 21450)
-- Dependencies: 218
-- Data for Name: DomainAggregate; Type: TABLE DATA; Schema: dbo; Owner: -
--

COPY dbo."DomainAggregate" ("DomainAggregateId", "SubContainerId", "TemplateId", "CategoryAttributeId", "Name", "Description", "CreatedByDomainIdentityId", "CreatedTime", "LastModifiedByDomainIdentityId", "LastModifiedTime") FROM stdin;
\.


--
-- TOC entry 4485 (class 0 OID 22332)
-- Dependencies: 235
-- Data for Name: DomainAggregateFlagAttribute; Type: TABLE DATA; Schema: dbo; Owner: -
--

COPY dbo."DomainAggregateFlagAttribute" ("DomainAggregateId", "FlagAttributeId") FROM stdin;
\.


--
-- TOC entry 4459 (class 0 OID 21150)
-- Dependencies: 209
-- Data for Name: DomainIdentity; Type: TABLE DATA; Schema: dbo; Owner: -
--

COPY dbo."DomainIdentity" ("DomainIdentityId", "UniqueIdentifier", "FirstName", "MiddleName", "LastName") FROM stdin;
705	vsts	King	T.	Animal
706	vsts2	Foo	J.	Bar
972	aecc3eb2-e0f1-4254-a8e2-aff9bc76614f	New First Name	Middle Name	New Last Name
12	cf93b1c9-3825-4136-80c1-795f4b24a9a2	New First Name	Middle Name	New Last Name
13	VssAdministrator	King	T.	Animal
74	f96b61f0-3697-4d51-95c2-02373d410bc1	New First Name	Middle Name	New Last Name
134	8ac1a8aa-ec5a-48b3-a9b1-659d07468af7	New First Name	Middle Name	New Last Name
214	844b3c54-5278-42ed-bee6-78501407e4ec	New First Name	Middle Name	New Last Name
298	55f78fd8-6457-4ee4-ad99-775066886c95	New First Name	Middle Name	New Last Name
25	6a00eb8d-f9cf-4af3-a0d5-b81551bc3369	New First Name	Middle Name	New Last Name
382	057ae432-1348-4206-b838-1dda5a2bdbf4	New First Name	Middle Name	New Last Name
86	4d4f455d-ffa4-470b-8190-e1dbc935fae1	New First Name	Middle Name	New Last Name
466	510e4612-766b-4bec-8c74-2c05154dbfca	New First Name	Middle Name	New Last Name
146	76f2aa88-d1fa-481d-9fe2-7a0eacbb9663	New First Name	Middle Name	New Last Name
550	864628f5-aae3-4e81-ae3d-7855ee9504b2	New First Name	Middle Name	New Last Name
634	ba61888f-8b29-4ad2-b3ec-848c7e6042be	New First Name	Middle Name	New Last Name
37	a1ae4bef-3bfa-4911-a74b-8c2d1ab9529d	New First Name	Middle Name	New Last Name
98	54d0255c-b2e1-4fb5-bdcd-c15e72a5fffe	New First Name	Middle Name	New Last Name
228	7b9eb5e7-9d21-4d1e-901c-db6a1ed0daed	New First Name	Middle Name	New Last Name
720	507cd099-d643-4f2b-917f-1497a257529b	New First Name	Middle Name	New Last Name
49	51914745-de36-42d7-9709-aba1b5391c69	New First Name	Middle Name	New Last Name
50	VssAdministrator2	Foo	J.	Bar
158	3d1f60be-d9ca-412b-8d54-6273be574573	New First Name	Middle Name	New Last Name
312	b277fb07-e4db-4da3-b9e3-fb0f01bddc47	New First Name	Middle Name	New Last Name
804	4669d5f5-ef97-46b3-ad71-ccc7a6dd5dc9	New First Name	Middle Name	New Last Name
396	f8bc68ca-5c02-4ba6-8f3e-6c0d5257a87f	New First Name	Middle Name	New Last Name
110	e13372f9-5a0d-4a03-bb55-c625a7ca0d33	New First Name	Middle Name	New Last Name
62	4cc16b2b-68c6-4edc-85cb-58118f355953	New First Name	Middle Name	New Last Name
480	e3287b69-4b02-4172-a3fb-d2d382777f7f	New First Name	Middle Name	New Last Name
242	bbccdae2-75ad-48b3-8eeb-a6e606bb3275	New First Name	Middle Name	New Last Name
172	8a59afbf-74a3-4b0a-b1e9-f8f45b7f7675	New First Name	Middle Name	New Last Name
122	7ba63103-2ce7-4723-ab5a-05335020a4cc	New First Name	Middle Name	New Last Name
564	35b0b006-c8b7-45db-b957-6ef3695af9ae	New First Name	Middle Name	New Last Name
326	55f03f59-a693-4db2-aca3-3a8091e0e789	New First Name	Middle Name	New Last Name
648	7a7964e1-e5c7-40eb-8b4c-995ae7954730	New First Name	Middle Name	New Last Name
410	d7c5e19a-805e-4646-90dd-fe882133de15	New First Name	Middle Name	New Last Name
186	c53e2477-1aff-42b0-b481-ca51f47d3c84	New First Name	Middle Name	New Last Name
256	6f1e51d7-c710-48a4-a819-7dff9c37c3b5	New First Name	Middle Name	New Last Name
494	300c3f5d-e8d1-4fb9-ae8d-d21b76acc305	New First Name	Middle Name	New Last Name
734	a0418050-5f9a-471b-aeff-cfa13bc92e39	New First Name	Middle Name	New Last Name
340	916e2fc8-234d-40ac-8d36-f45b798d7495	New First Name	Middle Name	New Last Name
200	6fa64e38-f2f3-4597-be9c-a5c778dc82d9	New First Name	Middle Name	New Last Name
578	a44a6551-a33b-47b7-a0cd-d38e77e17d59	New First Name	Middle Name	New Last Name
270	51eef36b-d66e-42c1-9d22-a2f284ae96df	New First Name	Middle Name	New Last Name
818	27d9475a-ffe9-4b14-ac70-41f98c585047	New First Name	Middle Name	New Last Name
424	3b7cf7c2-cee5-4f71-ab65-ca748598cfd5	New First Name	Middle Name	New Last Name
662	932f8f63-ec5a-4e3b-b051-d08333eaf7ea	New First Name	Middle Name	New Last Name
354	a8619f19-1584-42ab-94b7-b4ee644a940a	New First Name	Middle Name	New Last Name
508	645255a2-e2b7-47c0-a765-e16ccfc199bf	New First Name	Middle Name	New Last Name
284	6dd91e19-1d88-474c-bd7e-a2b0ecb65b49	New First Name	Middle Name	New Last Name
438	036ab206-b7be-4197-9aeb-5b5e088d10bc	New First Name	Middle Name	New Last Name
592	a29df330-7238-401d-9fa2-5ead41b65624	New First Name	Middle Name	New Last Name
368	b98a54a0-08e8-4b1b-a04c-58f7ae2b781e	New First Name	Middle Name	New Last Name
748	c4cffead-75e4-4db7-82ab-fab968e27b0c	New First Name	Middle Name	New Last Name
522	48eab29e-b762-4cbf-a9ba-d6a3cc85feb2	New First Name	Middle Name	New Last Name
676	3d93af23-b882-443f-b58e-69a3ef81fc1b	New First Name	Middle Name	New Last Name
452	cbcb493f-0f32-49df-a56c-801f2060f84b	New First Name	Middle Name	New Last Name
832	8caec354-de79-4c60-83d5-205605cfb610	New First Name	Middle Name	New Last Name
606	b1061e02-9eb5-4112-a335-094d17f8c7b9	New First Name	Middle Name	New Last Name
536	4dc72da5-efa2-4720-84d6-625876605c1a	New First Name	Middle Name	New Last Name
762	22555559-b2a8-4495-b8ba-3b327626c7c5	New First Name	Middle Name	New Last Name
690	b2fcccca-a312-4a0f-bcaf-cb2bd8046eda	New First Name	Middle Name	New Last Name
620	b22ecef9-69c1-49ff-97e4-6d251958ab67	New First Name	Middle Name	New Last Name
776	8bb6712e-8fcd-4dce-a812-abd75f3e3f6c	New First Name	Middle Name	New Last Name
704	db60b494-2151-4c4c-a5aa-3a16cc657b7a	New First Name	Middle Name	New Last Name
790	18ab4e86-6a7b-4028-a7cb-6e23a17e31a8	New First Name	Middle Name	New Last Name
1238	035d7b1f-9440-4304-885c-8744a5ed327a	New First Name	Middle Name	New Last Name
1490	02c70265-af4d-4d3f-9776-c54e376e59a8	New First Name	Middle Name	New Last Name
1322	f6ba6d63-6831-4a32-9860-3f07f573cf02	New First Name	Middle Name	New Last Name
846	591ca8f2-b212-483a-8d5b-799445d5ab18	New First Name	Middle Name	New Last Name
888	6e0b1421-eb20-461d-88ee-3fa01b472fc1	New First Name	Middle Name	New Last Name
986	ccac6542-7066-4286-9ce6-0afc68ea6bea	New First Name	Middle Name	New Last Name
1056	62913388-d186-4085-99a8-5b86e4dbfcf2	New First Name	Middle Name	New Last Name
1140	c95e5a26-d581-4193-9ae5-c985a2063529	New First Name	Middle Name	New Last Name
1252	8cfdee5e-d5a0-4394-b000-8f90b3fcb307	New First Name	Middle Name	New Last Name
860	a20c307a-6f8c-4deb-a695-93e561bc2521	New First Name	Middle Name	New Last Name
902	44d08762-aafe-45cd-aea5-7fb1fff01e7b	New First Name	Middle Name	New Last Name
1420	157773f1-27b2-4931-beaa-e9b22822b8e2	New First Name	Middle Name	New Last Name
1000	0d59f51e-8800-46b7-a8ce-51431b45967f	New First Name	Middle Name	New Last Name
1070	97cf0521-2b78-49de-816f-1581a14da361	New First Name	Middle Name	New Last Name
874	a1a685d7-1434-4dd4-bfa9-d3e6744b9f1d	New First Name	Middle Name	New Last Name
1154	eee13607-c813-424e-871a-8c0e8b8561ad	New First Name	Middle Name	New Last Name
916	11f52196-44c8-4c63-8cc5-42daecbd15cf	New First Name	Middle Name	New Last Name
1336	ba4bd95d-3e73-4907-8184-57478af6a229	New First Name	Middle Name	New Last Name
1266	28bd5264-51fc-498d-b14c-d5d4ad977de5	New First Name	Middle Name	New Last Name
1014	f0ea1684-4fbf-4bb8-95d7-c2baf4d45f1c	New First Name	Middle Name	New Last Name
1084	dab0dd03-7f34-430f-a1c7-7fc0a05c3895	New First Name	Middle Name	New Last Name
930	f4fa2431-6500-4e8f-882f-77ae8f5da96e	New First Name	Middle Name	New Last Name
1434	3d662c61-8d8a-4690-98fc-120040df8a3a	New First Name	Middle Name	New Last Name
1168	bb6027ab-e2af-44ae-a01e-abb83cd01b72	New First Name	Middle Name	New Last Name
1028	497d07c4-e4b8-4220-be25-b27037d2dfff	New First Name	Middle Name	New Last Name
944	d158ee5c-9ed3-47e3-9a72-5cdd69419137	New First Name	Middle Name	New Last Name
1280	d6268e77-35fd-40a8-9745-bc62223f1cec	New First Name	Middle Name	New Last Name
1098	77b41cdd-bf41-4814-b768-972372705cdc	New First Name	Middle Name	New Last Name
1350	067b3e54-d737-46bc-9657-a0f4b897cc6d	New First Name	Middle Name	New Last Name
1042	ea448518-17ff-46ac-8caa-931076a1de08	New First Name	Middle Name	New Last Name
958	1143209a-6de2-4152-b93f-bd4876f5a9d1	New First Name	Middle Name	New Last Name
1182	4dd88c0c-73c8-49df-acff-42df1755a03e	New First Name	Middle Name	New Last Name
1112	c527dcd8-8c5b-4e44-ae39-c066ef75c93d	New First Name	Middle Name	New Last Name
1448	f331f07b-dcba-439f-a1c0-3c0219786a76	New First Name	Middle Name	New Last Name
1294	de93e563-90c7-410a-9177-90355103beaa	New First Name	Middle Name	New Last Name
1364	bacc121d-28b3-4e6e-aed7-9653d8fee5cf	New First Name	Middle Name	New Last Name
1196	3afe77f8-ab9b-4bb6-a13c-4b82995eb46c	New First Name	Middle Name	New Last Name
1126	cb212f5f-65c9-42d4-897f-7ae6f8527f89	New First Name	Middle Name	New Last Name
1308	b38eeaa9-fb07-4c29-bbea-8f6b36109165	New First Name	Middle Name	New Last Name
1210	69f32267-7962-4486-b287-b36b4211a564	New First Name	Middle Name	New Last Name
1462	2f490087-febd-4957-979b-7809952cbe9a	New First Name	Middle Name	New Last Name
1378	38e3b4bc-d807-4737-a42e-ed56728bcf7c	New First Name	Middle Name	New Last Name
1224	fd406209-6472-4080-aec3-aee6b78848fc	New First Name	Middle Name	New Last Name
1476	8b6d3e05-ddaa-4910-9682-8aa7beee7d0a	New First Name	Middle Name	New Last Name
1392	da41c239-2bb3-4e89-b93f-fce582dda973	New First Name	Middle Name	New Last Name
1406	bc0e7b50-b363-4ea2-a289-a3afac3471e4	New First Name	Middle Name	New Last Name
\.


--
-- TOC entry 4461 (class 0 OID 21172)
-- Dependencies: 211
-- Data for Name: Field; Type: TABLE DATA; Schema: dbo; Owner: -
--

COPY dbo."Field" ("FieldId", "Name", "Description") FROM stdin;
1	UNIT_TEST-Field191553220	Mah Field Description
2	UNIT_TEST-Field1546378345	Mah Field Description
2877	UNIT_TEST-Field898624977	Mah Field Description
3048	UNIT_TEST-Field1100505367	Mah Field Description
3049	UNIT_TEST-Field861063196	Mah Field Description
908	UNIT_TEST-Field1481497062	Mah Field Description
3622	UNIT_TEST-Field1372182284	Mah Field Description
8	UNIT_TEST-Field1114256228	Mah Field Description The Second of That Name
3623	UNIT_TEST-Field1208921204	Mah Field Description
3868	UNIT_TEST-Field1941723598	Mah Field Description
3869	UNIT_TEST-Field733270976	Mah Field Description
909	UNIT_TEST-Field1546117878	Mah Field Description
2883	UNIT_TEST-Field974329667	Mah Field Description The Second of That Name
2884	UNIT_TEST-Field325819376	Mah Field Description
268	UNIT_TEST-Field1217658113	Mah Field Description
23	Internal ID	Unique ID used internally
24	First Name	The person's first name
25	Last Name	The person's last name
26	Yearly Wage	The base wage paid year over year.
27	Hire Date	The date and time of hire for the person
28	Bonus Target	The target bonus for the person
29	Contact Numbers	A list of contact numbers for the person in order of preference
30	UNIT_TEST-Field1210220554	Mah Field Description
31	UNIT_TEST-Field433114414	Mah Field Description
2885	UNIT_TEST-Field1903078875	Mah Field Description
104	UNIT_TEST-Field280593939	Mah Field Description
105	UNIT_TEST-Field59569805	Mah Field Description
37	UNIT_TEST-Field2131505966	Mah Field Description The Second of That Name
269	UNIT_TEST-Field1532869996	Mah Field Description
915	UNIT_TEST-Field1398788874	Mah Field Description The Second of That Name
916	UNIT_TEST-Field1261649446	Mah Field Description
327	UNIT_TEST-Field100122334	Mah Field Description The Second of That Name
111	UNIT_TEST-Field1518125077	Mah Field Description The Second of That Name
917	UNIT_TEST-Field1024819205	Mah Field Description
216	UNIT_TEST-Field274749178	Mah Field Description
217	UNIT_TEST-Field1631124995	Mah Field Description
320	UNIT_TEST-Field264868511	Mah Field Description
52	UNIT_TEST-Field62587377	Mah Field Description
53	UNIT_TEST-Field1806143425	Mah Field Description
321	UNIT_TEST-Field1047168848	Mah Field Description
223	UNIT_TEST-Field43208032	Mah Field Description The Second of That Name
424	UNIT_TEST-Field1177401639	Mah Field Description
59	UNIT_TEST-Field338251743	Mah Field Description The Second of That Name
425	UNIT_TEST-Field705177410	Mah Field Description
662	UNIT_TEST-Field1248696146	Mah Field Description
528	UNIT_TEST-Field187224467	Mah Field Description
529	UNIT_TEST-Field1681024662	Mah Field Description
663	UNIT_TEST-Field110194034	Mah Field Description
431	UNIT_TEST-Field1221406376	Mah Field Description The Second of That Name
923	UNIT_TEST-Field542434987	Mah Field Description The Second of That Name
535	UNIT_TEST-Field1581696988	Mah Field Description The Second of That Name
74	UNIT_TEST-Field407480145	Mah Field Description
75	UNIT_TEST-Field1512251881	Mah Field Description
669	UNIT_TEST-Field1962338602	Mah Field Description The Second of That Name
670	UNIT_TEST-Field866931363	Mah Field Description
134	UNIT_TEST-Field1945630074	Mah Field Description
135	UNIT_TEST-Field482092675	Mah Field Description
81	UNIT_TEST-Field935424853	Mah Field Description The Second of That Name
671	UNIT_TEST-Field1673854850	Mah Field Description
141	UNIT_TEST-Field1202432523	Mah Field Description The Second of That Name
275	UNIT_TEST-Field1577522773	Mah Field Description The Second of That Name
677	UNIT_TEST-Field503755817	Mah Field Description The Second of That Name
587	UNIT_TEST-Field1298335785	Mah Field Description The Second of That Name
588	UNIT_TEST-Field619197924	Mah Field Description
589	UNIT_TEST-Field747900850	Mah Field Description
164	UNIT_TEST-Field1593495656	Mah Field Description
165	UNIT_TEST-Field1322277366	Mah Field Description
372	UNIT_TEST-Field1739313105	Mah Field Description
373	UNIT_TEST-Field1492738292	Mah Field Description
171	UNIT_TEST-Field1098502012	Mah Field Description The Second of That Name
744	UNIT_TEST-Field718186908	Mah Field Description
745	UNIT_TEST-Field181688451	Mah Field Description
379	UNIT_TEST-Field4631740	Mah Field Description The Second of That Name
476	UNIT_TEST-Field1427368517	Mah Field Description
477	UNIT_TEST-Field1663152994	Mah Field Description
580	UNIT_TEST-Field1948526424	Mah Field Description
483	UNIT_TEST-Field1954573198	Mah Field Description The Second of That Name
581	UNIT_TEST-Field568008966	Mah Field Description
595	UNIT_TEST-Field1083459418	Mah Field Description The Second of That Name
751	UNIT_TEST-Field1844979194	Mah Field Description The Second of That Name
752	UNIT_TEST-Field1322654663	Mah Field Description
753	UNIT_TEST-Field307901786	Mah Field Description
759	UNIT_TEST-Field2105013559	Mah Field Description The Second of That Name
826	UNIT_TEST-Field1692029455	Mah Field Description
827	UNIT_TEST-Field1469083602	Mah Field Description
2891	UNIT_TEST-Field752699050	Mah Field Description The Second of That Name
1318	UNIT_TEST-Field588772835	Mah Field Description
1319	UNIT_TEST-Field1149489618	Mah Field Description
833	UNIT_TEST-Field1592550956	Mah Field Description The Second of That Name
834	UNIT_TEST-Field443975027	Mah Field Description
835	UNIT_TEST-Field1995153906	Mah Field Description
2712	UNIT_TEST-Field1056073058	Mah Field Description
1072	UNIT_TEST-Field2035296433	Mah Field Description
2220	UNIT_TEST-Field1220680224	Mah Field Description
1073	UNIT_TEST-Field1065345609	Mah Field Description
1236	UNIT_TEST-Field817431206	Mah Field Description
1237	UNIT_TEST-Field486586346	Mah Field Description
2221	UNIT_TEST-Field1060039560	Mah Field Description
2384	UNIT_TEST-Field1739220704	Mah Field Description
1400	UNIT_TEST-Field1370429300	Mah Field Description
1079	UNIT_TEST-Field778107848	Mah Field Description The Second of That Name
1080	UNIT_TEST-Field2069024287	Mah Field Description
841	UNIT_TEST-Field828324813	Mah Field Description The Second of That Name
1081	UNIT_TEST-Field864952222	Mah Field Description
1401	UNIT_TEST-Field905872837	Mah Field Description
1564	UNIT_TEST-Field930127423	Mah Field Description
1243	UNIT_TEST-Field726693712	Mah Field Description The Second of That Name
1244	UNIT_TEST-Field2117500801	Mah Field Description
1087	UNIT_TEST-Field2012480717	Mah Field Description The Second of That Name
1245	UNIT_TEST-Field1520043278	Mah Field Description
1565	UNIT_TEST-Field987868979	Mah Field Description
1728	UNIT_TEST-Field825376239	Mah Field Description
1729	UNIT_TEST-Field1311229102	Mah Field Description
1892	UNIT_TEST-Field1973844915	Mah Field Description
1251	UNIT_TEST-Field1355262296	Mah Field Description The Second of That Name
1407	UNIT_TEST-Field1197908684	Mah Field Description The Second of That Name
1408	UNIT_TEST-Field295973328	Mah Field Description
1409	UNIT_TEST-Field256266225	Mah Field Description
1893	UNIT_TEST-Field1803870555	Mah Field Description
2385	UNIT_TEST-Field49928827	Mah Field Description
990	UNIT_TEST-Field951107583	Mah Field Description
991	UNIT_TEST-Field471352442	Mah Field Description
1571	UNIT_TEST-Field1140015579	Mah Field Description The Second of That Name
1572	UNIT_TEST-Field1211810287	Mah Field Description
1415	UNIT_TEST-Field551935895	Mah Field Description The Second of That Name
997	UNIT_TEST-Field1170169020	Mah Field Description The Second of That Name
998	UNIT_TEST-Field1273100045	Mah Field Description
999	UNIT_TEST-Field1380544330	Mah Field Description
1573	UNIT_TEST-Field1491695407	Mah Field Description
2713	UNIT_TEST-Field1887281063	Mah Field Description
3055	UNIT_TEST-Field874338967	Mah Field Description The Second of That Name
1735	UNIT_TEST-Field1470507540	Mah Field Description The Second of That Name
1736	UNIT_TEST-Field2081203831	Mah Field Description
1579	UNIT_TEST-Field999925126	Mah Field Description The Second of That Name
1737	UNIT_TEST-Field489543338	Mah Field Description
3629	UNIT_TEST-Field106201817	Mah Field Description The Second of That Name
1005	UNIT_TEST-Field501455599	Mah Field Description The Second of That Name
1489	UNIT_TEST-Field715602879	Mah Field Description The Second of That Name
1743	UNIT_TEST-Field1891593591	Mah Field Description The Second of That Name
1154	UNIT_TEST-Field1614797688	Mah Field Description
1155	UNIT_TEST-Field2094283506	Mah Field Description
1490	UNIT_TEST-Field1129984826	Mah Field Description
1491	UNIT_TEST-Field653009539	Mah Field Description
1161	UNIT_TEST-Field353637102	Mah Field Description The Second of That Name
1162	UNIT_TEST-Field301528006	Mah Field Description
1163	UNIT_TEST-Field2121789273	Mah Field Description
1169	UNIT_TEST-Field731153796	Mah Field Description The Second of That Name
1325	UNIT_TEST-Field1819678227	Mah Field Description The Second of That Name
1326	UNIT_TEST-Field724951441	Mah Field Description
1327	UNIT_TEST-Field475546318	Mah Field Description
1333	UNIT_TEST-Field622734803	Mah Field Description The Second of That Name
1482	UNIT_TEST-Field151420360	Mah Field Description
1483	UNIT_TEST-Field1649816750	Mah Field Description
1810	UNIT_TEST-Field470062866	Mah Field Description
1811	UNIT_TEST-Field1480102456	Mah Field Description
1497	UNIT_TEST-Field527874690	Mah Field Description The Second of That Name
1646	UNIT_TEST-Field28383009	Mah Field Description
1647	UNIT_TEST-Field1380554107	Mah Field Description
1653	UNIT_TEST-Field389964735	Mah Field Description The Second of That Name
1654	UNIT_TEST-Field1750094668	Mah Field Description
1655	UNIT_TEST-Field1654253250	Mah Field Description
1661	UNIT_TEST-Field360461103	Mah Field Description The Second of That Name
1817	UNIT_TEST-Field742768021	Mah Field Description The Second of That Name
1818	UNIT_TEST-Field1828439691	Mah Field Description
1819	UNIT_TEST-Field405367773	Mah Field Description
1825	UNIT_TEST-Field2093730132	Mah Field Description The Second of That Name
2056	UNIT_TEST-Field469051706	Mah Field Description
1899	UNIT_TEST-Field744802173	Mah Field Description The Second of That Name
1900	UNIT_TEST-Field1086488287	Mah Field Description
1901	UNIT_TEST-Field650250197	Mah Field Description
2057	UNIT_TEST-Field1219405864	Mah Field Description
2548	UNIT_TEST-Field1051080157	Mah Field Description
1907	UNIT_TEST-Field1266276078	Mah Field Description The Second of That Name
2549	UNIT_TEST-Field892196146	Mah Field Description
2063	UNIT_TEST-Field1803121634	Mah Field Description The Second of That Name
2064	UNIT_TEST-Field1272581521	Mah Field Description
2065	UNIT_TEST-Field1466152844	Mah Field Description
2227	UNIT_TEST-Field478472131	Mah Field Description The Second of That Name
2228	UNIT_TEST-Field1444721470	Mah Field Description
2229	UNIT_TEST-Field891141568	Mah Field Description
2071	UNIT_TEST-Field129232437	Mah Field Description The Second of That Name
3875	UNIT_TEST-Field1674263407	Mah Field Description The Second of That Name
2391	UNIT_TEST-Field1637655847	Mah Field Description The Second of That Name
2392	UNIT_TEST-Field403775542	Mah Field Description
2235	UNIT_TEST-Field1121075552	Mah Field Description The Second of That Name
2393	UNIT_TEST-Field416794065	Mah Field Description
99	MERGE_Existing_Yearly Wage	The base wage paid year over year.
2555	UNIT_TEST-Field821929455	Mah Field Description The Second of That Name
2399	UNIT_TEST-Field537881653	Mah Field Description The Second of That Name
2556	UNIT_TEST-Field2111857772	Mah Field Description
2557	UNIT_TEST-Field1375223583	Mah Field Description
3286	UNIT_TEST-Field1985980862	Mah Field Description
2719	UNIT_TEST-Field733138504	Mah Field Description The Second of That Name
2720	UNIT_TEST-Field1756588517	Mah Field Description
2721	UNIT_TEST-Field478459001	Mah Field Description
2563	UNIT_TEST-Field588814311	Mah Field Description The Second of That Name
98	MERGE_Existing_Last Name	The person's last name
3450	UNIT_TEST-Field685770327	Mah Field Description
2466	UNIT_TEST-Field1660391091	Mah Field Description
2467	UNIT_TEST-Field1573156013	Mah Field Description
2727	UNIT_TEST-Field1246450982	Mah Field Description The Second of That Name
3287	UNIT_TEST-Field1585348638	Mah Field Description
3451	UNIT_TEST-Field1191849887	Mah Field Description
101	MERGE_NonExisting_Hire Date	The date and time of hire for the person
1974	UNIT_TEST-Field1150317838	Mah Field Description
1975	UNIT_TEST-Field1427712837	Mah Field Description
102	MERGE_NonExisting_Bonus Target	The target bonus for the person
1981	UNIT_TEST-Field733279586	Mah Field Description The Second of That Name
1982	UNIT_TEST-Field1907870482	Mah Field Description
1983	UNIT_TEST-Field684667817	Mah Field Description
2302	UNIT_TEST-Field1940322015	Mah Field Description
2303	UNIT_TEST-Field400058177	Mah Field Description
2138	UNIT_TEST-Field1025673921	Mah Field Description
1989	UNIT_TEST-Field412594330	Mah Field Description The Second of That Name
2139	UNIT_TEST-Field2143598048	Mah Field Description
2145	UNIT_TEST-Field292908522	Mah Field Description The Second of That Name
2146	UNIT_TEST-Field1818267144	Mah Field Description
2147	UNIT_TEST-Field427799495	Mah Field Description
2309	UNIT_TEST-Field2010992620	Mah Field Description The Second of That Name
2310	UNIT_TEST-Field478147850	Mah Field Description
2311	UNIT_TEST-Field1032806729	Mah Field Description
2153	UNIT_TEST-Field237617198	Mah Field Description The Second of That Name
2317	UNIT_TEST-Field562345644	Mah Field Description The Second of That Name
2637	UNIT_TEST-Field1120828525	Mah Field Description The Second of That Name
2473	UNIT_TEST-Field338238515	Mah Field Description The Second of That Name
2474	UNIT_TEST-Field1281684708	Mah Field Description
2475	UNIT_TEST-Field1269891182	Mah Field Description
2630	UNIT_TEST-Field1138281256	Mah Field Description
2631	UNIT_TEST-Field1939024777	Mah Field Description
2481	UNIT_TEST-Field930063231	Mah Field Description The Second of That Name
2638	UNIT_TEST-Field219156714	Mah Field Description
2639	UNIT_TEST-Field1053217063	Mah Field Description
2645	UNIT_TEST-Field48065843	Mah Field Description The Second of That Name
2794	UNIT_TEST-Field1187273311	Mah Field Description
2795	UNIT_TEST-Field805466053	Mah Field Description
2801	UNIT_TEST-Field656382615	Mah Field Description The Second of That Name
2802	UNIT_TEST-Field1638529891	Mah Field Description
2803	UNIT_TEST-Field1359680147	Mah Field Description
2809	UNIT_TEST-Field1062071540	Mah Field Description The Second of That Name
2876	UNIT_TEST-Field1423990164	Mah Field Description
3293	UNIT_TEST-Field1177143579	Mah Field Description The Second of That Name
3294	UNIT_TEST-Field1838401288	Mah Field Description
3295	UNIT_TEST-Field1956859642	Mah Field Description
103	MERGE_NonExisting_Contact Numbers	A list of contact numbers for the person in order of preference
3457	UNIT_TEST-Field702976213	Mah Field Description The Second of That Name
3458	UNIT_TEST-Field1018383338	Mah Field Description
3459	UNIT_TEST-Field1141079180	Mah Field Description
3301	UNIT_TEST-Field2106729639	Mah Field Description The Second of That Name
4106	UNIT_TEST-Field133811849	Mah Field Description
3465	UNIT_TEST-Field1700611050	Mah Field Description The Second of That Name
4025	UNIT_TEST-Field1979600095	Mah Field Description
4107	UNIT_TEST-Field1895597243	Mah Field Description
4270	UNIT_TEST-Field583622791	Mah Field Description
4271	UNIT_TEST-Field693020129	Mah Field Description
4113	UNIT_TEST-Field22172267	Mah Field Description The Second of That Name
3793	UNIT_TEST-Field1938159966	Mah Field Description The Second of That Name
4114	UNIT_TEST-Field264408365	Mah Field Description
3204	UNIT_TEST-Field907232477	Mah Field Description
3205	UNIT_TEST-Field1116345109	Mah Field Description
4115	UNIT_TEST-Field1104032282	Mah Field Description
3211	UNIT_TEST-Field1974465604	Mah Field Description The Second of That Name
3212	UNIT_TEST-Field940702852	Mah Field Description
3213	UNIT_TEST-Field1177576486	Mah Field Description
4277	UNIT_TEST-Field1830583301	Mah Field Description The Second of That Name
4278	UNIT_TEST-Field776436648	Mah Field Description
4121	UNIT_TEST-Field1952669138	Mah Field Description The Second of That Name
4279	UNIT_TEST-Field719444131	Mah Field Description
3219	UNIT_TEST-Field1520273662	Mah Field Description The Second of That Name
4031	UNIT_TEST-Field1158221573	Mah Field Description The Second of That Name
4032	UNIT_TEST-Field1796690013	Mah Field Description
3621	UNIT_TEST-Field1423402592	Mah Field Description The Second of That Name
4033	UNIT_TEST-Field251467596	Mah Field Description
3778	UNIT_TEST-Field692572061	Mah Field Description
3779	UNIT_TEST-Field1725431939	Mah Field Description
4285	UNIT_TEST-Field1998793928	Mah Field Description The Second of That Name
3785	UNIT_TEST-Field1201698683	Mah Field Description The Second of That Name
3786	UNIT_TEST-Field1709385454	Mah Field Description
3787	UNIT_TEST-Field922179252	Mah Field Description
3614	UNIT_TEST-Field1421238738	Mah Field Description
3615	UNIT_TEST-Field62710733	Mah Field Description
4024	UNIT_TEST-Field1883479003	Mah Field Description
4039	UNIT_TEST-Field1201033126	Mah Field Description The Second of That Name
96	MERGE_Existing_Internal ID	Unique ID used internally
97	MERGE_Existing_First Name	The person's first name
2958	UNIT_TEST-Field1993333589	Mah Field Description
2959	UNIT_TEST-Field1240960212	Mah Field Description
3122	UNIT_TEST-Field1588032487	Mah Field Description
2965	UNIT_TEST-Field1888145698	Mah Field Description The Second of That Name
2966	UNIT_TEST-Field1056568717	Mah Field Description
2967	UNIT_TEST-Field1898833983	Mah Field Description
3123	UNIT_TEST-Field585334833	Mah Field Description
3368	UNIT_TEST-Field1891624948	Mah Field Description
3369	UNIT_TEST-Field351694824	Mah Field Description
3532	UNIT_TEST-Field2113316547	Mah Field Description
3533	UNIT_TEST-Field1077939635	Mah Field Description
2973	UNIT_TEST-Field28545006	Mah Field Description The Second of That Name
3696	UNIT_TEST-Field589328945	Mah Field Description
3129	UNIT_TEST-Field391698344	Mah Field Description The Second of That Name
3130	UNIT_TEST-Field1878869154	Mah Field Description
3131	UNIT_TEST-Field1477807352	Mah Field Description
3697	UNIT_TEST-Field1001567553	Mah Field Description
3942	UNIT_TEST-Field1019077139	Mah Field Description
3375	UNIT_TEST-Field1937877523	Mah Field Description The Second of That Name
3376	UNIT_TEST-Field918144268	Mah Field Description
3137	UNIT_TEST-Field922448594	Mah Field Description The Second of That Name
3377	UNIT_TEST-Field128885065	Mah Field Description
3943	UNIT_TEST-Field693239555	Mah Field Description
4188	UNIT_TEST-Field1868768312	Mah Field Description
3539	UNIT_TEST-Field1938157888	Mah Field Description The Second of That Name
3540	UNIT_TEST-Field1523903183	Mah Field Description
3383	UNIT_TEST-Field557714662	Mah Field Description The Second of That Name
3541	UNIT_TEST-Field202278510	Mah Field Description
4189	UNIT_TEST-Field383094339	Mah Field Description
4352	UNIT_TEST-Field1964439785	Mah Field Description
3703	UNIT_TEST-Field795976946	Mah Field Description The Second of That Name
3704	UNIT_TEST-Field1022162910	Mah Field Description
3547	UNIT_TEST-Field1961703471	Mah Field Description The Second of That Name
3705	UNIT_TEST-Field2054897584	Mah Field Description
4353	UNIT_TEST-Field2137244723	Mah Field Description
3949	UNIT_TEST-Field958447184	Mah Field Description The Second of That Name
3950	UNIT_TEST-Field899396205	Mah Field Description
3711	UNIT_TEST-Field1608029930	Mah Field Description The Second of That Name
3951	UNIT_TEST-Field1341228405	Mah Field Description
4195	UNIT_TEST-Field1385187348	Mah Field Description The Second of That Name
4196	UNIT_TEST-Field19918470	Mah Field Description
3040	UNIT_TEST-Field987605037	Mah Field Description
3041	UNIT_TEST-Field1853068738	Mah Field Description
3957	UNIT_TEST-Field1355738264	Mah Field Description The Second of That Name
4197	UNIT_TEST-Field1315486115	Mah Field Description
3047	UNIT_TEST-Field342109449	Mah Field Description The Second of That Name
4359	UNIT_TEST-Field1802912930	Mah Field Description The Second of That Name
4360	UNIT_TEST-Field724427709	Mah Field Description
4203	UNIT_TEST-Field2108640475	Mah Field Description The Second of That Name
4361	UNIT_TEST-Field473061328	Mah Field Description
4367	UNIT_TEST-Field868207461	Mah Field Description The Second of That Name
3860	UNIT_TEST-Field881292224	Mah Field Description
3861	UNIT_TEST-Field637043536	Mah Field Description
3867	UNIT_TEST-Field791031451	Mah Field Description The Second of That Name
4397	INS_Internal ID	Unique ID used internally
4398	INS_First Name	The person's first name
4399	INS_Last Name	The person's last name
4400	INS_Yearly Wage	The base wage paid year over year.
4401	INS_Hire Date	The date and time of hire for the person
4402	INS_Bonus Target	The target bonus for the person
4403	INS_Contact Numbers	A list of contact numbers for the person in order of preference
\.


--
-- TOC entry 4477 (class 0 OID 21776)
-- Dependencies: 227
-- Data for Name: FieldValue; Type: TABLE DATA; Schema: dbo; Owner: -
--

COPY dbo."FieldValue" ("FieldValueId", "FieldId", "LastModifiedByDomainIdentifierId", "LastModifiedTime") FROM stdin;
337	23	13	2020-10-15 23:43:37.987+00
338	24	13	2020-10-15 23:43:37.987+00
339	25	13	2020-10-15 23:43:37.987+00
340	26	13	2020-10-15 23:43:37.987+00
8	23	13	2020-08-14 00:35:45.207962+00
9	24	13	2020-08-14 00:35:45.207962+00
10	25	13	2020-08-14 00:35:45.207962+00
11	26	13	2020-08-14 00:35:45.207962+00
12	27	13	2020-08-14 00:35:45.207962+00
13	28	13	2020-08-14 00:35:45.207962+00
14	29	13	2020-08-14 00:35:45.207962+00
15	23	13	2020-08-14 00:46:14.307761+00
16	24	13	2020-08-14 00:46:14.307761+00
17	25	13	2020-08-14 00:46:14.307761+00
18	26	13	2020-08-14 00:46:14.307761+00
19	27	13	2020-08-14 00:46:14.307761+00
20	28	13	2020-08-14 00:46:14.307761+00
21	29	13	2020-08-14 00:46:14.307761+00
22	23	13	2020-08-17 00:45:49.305+00
23	24	13	2020-08-17 00:45:49.305+00
24	25	13	2020-08-17 00:45:49.305+00
25	26	13	2020-08-17 00:45:49.305+00
26	27	13	2020-08-17 00:45:49.305+00
27	28	13	2020-08-17 00:45:49.305+00
28	29	13	2020-08-17 00:45:49.305+00
33	99	50	2020-08-17 00:45:51.022+00
34	101	50	2020-08-17 00:45:51.022+00
35	102	50	2020-08-17 00:45:51.022+00
36	103	50	2020-08-17 00:45:51.022+00
37	23	13	2020-08-17 22:47:25.655+00
38	24	13	2020-08-17 22:47:25.655+00
39	25	13	2020-08-17 22:47:25.655+00
40	26	13	2020-08-17 22:47:25.655+00
41	27	13	2020-08-17 22:47:25.655+00
42	28	13	2020-08-17 22:47:25.655+00
43	29	13	2020-08-17 22:47:25.655+00
48	99	50	2020-08-17 22:47:26.346+00
49	101	50	2020-08-17 22:47:26.346+00
50	102	50	2020-08-17 22:47:26.346+00
51	103	50	2020-08-17 22:47:26.346+00
52	23	13	2020-08-20 21:10:07.62+00
53	24	13	2020-08-20 21:10:07.62+00
54	25	13	2020-08-20 21:10:07.62+00
55	26	13	2020-08-20 21:10:07.62+00
56	27	13	2020-08-20 21:10:07.62+00
57	28	13	2020-08-20 21:10:07.62+00
58	29	13	2020-08-20 21:10:07.62+00
63	99	50	2020-08-20 21:10:08.777+00
64	101	50	2020-08-20 21:10:08.777+00
65	102	50	2020-08-20 21:10:08.777+00
66	103	50	2020-08-20 21:10:08.777+00
78	99	50	2020-08-20 21:10:13.55+00
79	101	50	2020-08-20 21:10:13.55+00
80	102	50	2020-08-20 21:10:13.55+00
81	103	50	2020-08-20 21:10:13.55+00
82	23	13	2020-08-21 02:12:13.761+00
83	24	13	2020-08-21 02:12:13.761+00
84	25	13	2020-08-21 02:12:13.761+00
85	26	13	2020-08-21 02:12:13.761+00
86	27	13	2020-08-21 02:12:13.761+00
87	28	13	2020-08-21 02:12:13.761+00
88	29	13	2020-08-21 02:12:13.761+00
93	99	50	2020-08-21 02:12:14.047+00
94	101	50	2020-08-21 02:12:14.047+00
95	102	50	2020-08-21 02:12:14.047+00
96	103	50	2020-08-21 02:12:14.047+00
97	23	13	2020-08-21 02:12:15.91+00
98	24	13	2020-08-21 02:12:15.91+00
99	25	13	2020-08-21 02:12:15.91+00
100	26	13	2020-08-21 02:12:15.91+00
101	27	13	2020-08-21 02:12:15.91+00
102	28	13	2020-08-21 02:12:15.91+00
103	29	13	2020-08-21 02:12:15.91+00
108	99	50	2020-08-21 02:12:16.19+00
109	101	50	2020-08-21 02:12:16.19+00
110	102	50	2020-08-21 02:12:16.19+00
111	103	50	2020-08-21 02:12:16.19+00
112	23	13	2020-08-25 18:43:20.163+00
113	24	13	2020-08-25 18:43:20.163+00
114	25	13	2020-08-25 18:43:20.163+00
115	26	13	2020-08-25 18:43:20.163+00
116	27	13	2020-08-25 18:43:20.163+00
117	28	13	2020-08-25 18:43:20.163+00
118	29	13	2020-08-25 18:43:20.163+00
123	99	50	2020-08-25 18:43:20.66+00
124	101	50	2020-08-25 18:43:20.66+00
125	102	50	2020-08-25 18:43:20.66+00
126	103	50	2020-08-25 18:43:20.66+00
127	23	13	2020-08-25 18:43:23.217+00
128	24	13	2020-08-25 18:43:23.217+00
129	25	13	2020-08-25 18:43:23.217+00
130	26	13	2020-08-25 18:43:23.217+00
131	27	13	2020-08-25 18:43:23.217+00
132	28	13	2020-08-25 18:43:23.217+00
133	29	13	2020-08-25 18:43:23.217+00
138	99	50	2020-08-25 18:43:23.56+00
139	101	50	2020-08-25 18:43:23.56+00
140	102	50	2020-08-25 18:43:23.56+00
141	103	50	2020-08-25 18:43:23.56+00
142	23	13	2020-08-26 00:45:47.581+00
143	24	13	2020-08-26 00:45:47.581+00
144	25	13	2020-08-26 00:45:47.581+00
145	26	13	2020-08-26 00:45:47.581+00
146	27	13	2020-08-26 00:45:47.581+00
147	28	13	2020-08-26 00:45:47.581+00
148	29	13	2020-08-26 00:45:47.581+00
153	99	50	2020-08-26 00:45:48.039+00
154	101	50	2020-08-26 00:45:48.039+00
155	102	50	2020-08-26 00:45:48.039+00
156	103	50	2020-08-26 00:45:48.039+00
157	23	13	2020-08-26 00:45:50.445+00
158	24	13	2020-08-26 00:45:50.445+00
159	25	13	2020-08-26 00:45:50.445+00
160	26	13	2020-08-26 00:45:50.445+00
161	27	13	2020-08-26 00:45:50.445+00
162	28	13	2020-08-26 00:45:50.445+00
163	29	13	2020-08-26 00:45:50.445+00
168	99	50	2020-08-26 00:45:50.71+00
169	101	50	2020-08-26 00:45:50.71+00
170	102	50	2020-08-26 00:45:50.71+00
171	103	50	2020-08-26 00:45:50.71+00
172	23	13	2020-08-28 14:57:21.511+00
173	24	13	2020-08-28 14:57:21.511+00
174	25	13	2020-08-28 14:57:21.511+00
175	26	13	2020-08-28 14:57:21.511+00
176	27	13	2020-08-28 14:57:21.511+00
177	28	13	2020-08-28 14:57:21.511+00
178	29	13	2020-08-28 14:57:21.511+00
183	99	50	2020-08-28 14:57:23.727+00
184	101	50	2020-08-28 14:57:23.727+00
185	102	50	2020-08-28 14:57:23.727+00
186	103	50	2020-08-28 14:57:23.727+00
187	23	13	2020-08-28 14:57:42.618+00
188	24	13	2020-08-28 14:57:42.618+00
189	25	13	2020-08-28 14:57:42.618+00
190	26	13	2020-08-28 14:57:42.618+00
191	27	13	2020-08-28 14:57:42.618+00
192	28	13	2020-08-28 14:57:42.618+00
193	29	13	2020-08-28 14:57:42.618+00
198	99	50	2020-08-28 14:57:44.528+00
199	101	50	2020-08-28 14:57:44.528+00
200	102	50	2020-08-28 14:57:44.528+00
201	103	50	2020-08-28 14:57:44.528+00
202	23	13	2020-09-17 02:06:04.172+00
203	24	13	2020-09-17 02:06:04.172+00
204	25	13	2020-09-17 02:06:04.172+00
205	26	13	2020-09-17 02:06:04.172+00
206	27	13	2020-09-17 02:06:04.172+00
207	28	13	2020-09-17 02:06:04.172+00
208	29	13	2020-09-17 02:06:04.172+00
213	99	50	2020-09-17 02:06:04.704+00
214	101	50	2020-09-17 02:06:04.704+00
215	102	50	2020-09-17 02:06:04.704+00
216	103	50	2020-09-17 02:06:04.704+00
217	23	13	2020-09-17 02:06:07.48+00
218	24	13	2020-09-17 02:06:07.48+00
219	25	13	2020-09-17 02:06:07.48+00
220	26	13	2020-09-17 02:06:07.48+00
221	27	13	2020-09-17 02:06:07.48+00
222	28	13	2020-09-17 02:06:07.48+00
223	29	13	2020-09-17 02:06:07.48+00
341	27	13	2020-10-15 23:43:37.987+00
342	28	13	2020-10-15 23:43:37.987+00
343	29	13	2020-10-15 23:43:37.987+00
1027	23	13	2021-01-28 19:08:04.044+00
228	99	50	2020-09-17 02:06:07.713+00
229	101	50	2020-09-17 02:06:07.713+00
230	102	50	2020-09-17 02:06:07.713+00
231	103	50	2020-09-17 02:06:07.713+00
232	23	13	2020-10-01 16:38:18.693+00
233	24	13	2020-10-01 16:38:18.693+00
234	25	13	2020-10-01 16:38:18.693+00
235	26	13	2020-10-01 16:38:18.693+00
236	27	13	2020-10-01 16:38:18.693+00
237	28	13	2020-10-01 16:38:18.693+00
238	29	13	2020-10-01 16:38:18.693+00
1028	24	13	2021-01-28 19:08:04.044+00
1029	25	13	2021-01-28 19:08:04.044+00
1030	26	13	2021-01-28 19:08:04.044+00
348	99	50	2020-10-15 23:43:38.541+00
243	99	50	2020-10-01 16:38:19.741+00
244	101	50	2020-10-01 16:38:19.741+00
245	102	50	2020-10-01 16:38:19.741+00
246	103	50	2020-10-01 16:38:19.741+00
247	23	13	2020-10-01 16:38:23.546+00
248	24	13	2020-10-01 16:38:23.546+00
249	25	13	2020-10-01 16:38:23.546+00
250	26	13	2020-10-01 16:38:23.546+00
251	27	13	2020-10-01 16:38:23.546+00
252	28	13	2020-10-01 16:38:23.546+00
253	29	13	2020-10-01 16:38:23.546+00
349	101	50	2020-10-15 23:43:38.541+00
350	102	50	2020-10-15 23:43:38.541+00
351	103	50	2020-10-15 23:43:38.541+00
352	23	13	2020-10-15 23:43:43.484+00
258	99	50	2020-10-01 16:38:23.849+00
259	101	50	2020-10-01 16:38:23.849+00
260	102	50	2020-10-01 16:38:23.849+00
261	103	50	2020-10-01 16:38:23.849+00
262	23	13	2020-10-11 00:42:53.085+00
263	24	13	2020-10-11 00:42:53.085+00
264	25	13	2020-10-11 00:42:53.085+00
265	26	13	2020-10-11 00:42:53.085+00
266	27	13	2020-10-11 00:42:53.085+00
267	28	13	2020-10-11 00:42:53.085+00
268	29	13	2020-10-11 00:42:53.085+00
353	24	13	2020-10-15 23:43:43.484+00
354	25	13	2020-10-15 23:43:43.484+00
355	26	13	2020-10-15 23:43:43.484+00
356	27	13	2020-10-15 23:43:43.484+00
273	99	50	2020-10-11 00:42:53.565+00
274	101	50	2020-10-11 00:42:53.565+00
275	102	50	2020-10-11 00:42:53.565+00
276	103	50	2020-10-11 00:42:53.565+00
277	23	13	2020-10-11 00:42:56.165+00
278	24	13	2020-10-11 00:42:56.165+00
279	25	13	2020-10-11 00:42:56.165+00
280	26	13	2020-10-11 00:42:56.165+00
281	27	13	2020-10-11 00:42:56.165+00
282	28	13	2020-10-11 00:42:56.165+00
283	29	13	2020-10-11 00:42:56.165+00
357	28	13	2020-10-15 23:43:43.484+00
358	29	13	2020-10-15 23:43:43.484+00
288	99	50	2020-10-11 00:42:56.354+00
289	101	50	2020-10-11 00:42:56.354+00
290	102	50	2020-10-11 00:42:56.354+00
291	103	50	2020-10-11 00:42:56.354+00
292	23	13	2020-10-15 20:51:37.561+00
293	24	13	2020-10-15 20:51:37.561+00
294	25	13	2020-10-15 20:51:37.561+00
295	26	13	2020-10-15 20:51:37.561+00
296	27	13	2020-10-15 20:51:37.561+00
297	28	13	2020-10-15 20:51:37.561+00
298	29	13	2020-10-15 20:51:37.561+00
363	99	50	2020-10-15 23:43:43.684+00
364	101	50	2020-10-15 23:43:43.684+00
303	99	50	2020-10-15 20:51:38.205+00
304	101	50	2020-10-15 20:51:38.205+00
305	102	50	2020-10-15 20:51:38.205+00
306	103	50	2020-10-15 20:51:38.205+00
307	23	13	2020-10-15 20:51:44.228+00
308	24	13	2020-10-15 20:51:44.228+00
309	25	13	2020-10-15 20:51:44.228+00
310	26	13	2020-10-15 20:51:44.228+00
311	27	13	2020-10-15 20:51:44.228+00
312	28	13	2020-10-15 20:51:44.228+00
313	29	13	2020-10-15 20:51:44.228+00
365	102	50	2020-10-15 23:43:43.684+00
366	103	50	2020-10-15 23:43:43.684+00
367	23	13	2020-10-15 23:43:43.95+00
368	24	13	2020-10-15 23:43:43.95+00
318	99	50	2020-10-15 20:51:44.502+00
319	101	50	2020-10-15 20:51:44.502+00
320	102	50	2020-10-15 20:51:44.502+00
321	103	50	2020-10-15 20:51:44.502+00
322	23	13	2020-10-15 20:51:44.942+00
323	24	13	2020-10-15 20:51:44.942+00
324	25	13	2020-10-15 20:51:44.942+00
325	26	13	2020-10-15 20:51:44.942+00
326	27	13	2020-10-15 20:51:44.942+00
327	28	13	2020-10-15 20:51:44.942+00
328	29	13	2020-10-15 20:51:44.942+00
369	25	13	2020-10-15 23:43:43.95+00
370	26	13	2020-10-15 23:43:43.95+00
371	27	13	2020-10-15 23:43:43.95+00
372	28	13	2020-10-15 23:43:43.95+00
333	99	50	2020-10-15 20:51:45.31+00
334	101	50	2020-10-15 20:51:45.31+00
335	102	50	2020-10-15 20:51:45.31+00
336	103	50	2020-10-15 20:51:45.31+00
373	29	13	2020-10-15 23:43:43.95+00
378	99	50	2020-10-15 23:43:44.17+00
379	101	50	2020-10-15 23:43:44.17+00
380	102	50	2020-10-15 23:43:44.17+00
381	103	50	2020-10-15 23:43:44.17+00
382	23	13	2020-10-16 18:37:03.803+00
383	24	13	2020-10-16 18:37:03.803+00
384	25	13	2020-10-16 18:37:03.803+00
385	26	13	2020-10-16 18:37:03.803+00
386	27	13	2020-10-16 18:37:03.803+00
387	28	13	2020-10-16 18:37:03.803+00
388	29	13	2020-10-16 18:37:03.803+00
393	99	50	2020-10-16 18:37:04.204+00
394	101	50	2020-10-16 18:37:04.204+00
395	102	50	2020-10-16 18:37:04.204+00
396	103	50	2020-10-16 18:37:04.204+00
397	23	13	2020-10-16 18:37:08.701+00
398	24	13	2020-10-16 18:37:08.701+00
399	25	13	2020-10-16 18:37:08.701+00
400	26	13	2020-10-16 18:37:08.701+00
401	27	13	2020-10-16 18:37:08.701+00
402	28	13	2020-10-16 18:37:08.701+00
403	29	13	2020-10-16 18:37:08.701+00
408	99	50	2020-10-16 18:37:08.91+00
409	101	50	2020-10-16 18:37:08.91+00
410	102	50	2020-10-16 18:37:08.91+00
411	103	50	2020-10-16 18:37:08.91+00
412	23	13	2020-10-16 18:37:09.177+00
413	24	13	2020-10-16 18:37:09.177+00
414	25	13	2020-10-16 18:37:09.177+00
415	26	13	2020-10-16 18:37:09.177+00
416	27	13	2020-10-16 18:37:09.177+00
417	28	13	2020-10-16 18:37:09.177+00
418	29	13	2020-10-16 18:37:09.177+00
423	99	50	2020-10-16 18:37:09.408+00
424	101	50	2020-10-16 18:37:09.408+00
425	102	50	2020-10-16 18:37:09.408+00
426	103	50	2020-10-16 18:37:09.408+00
427	23	13	2020-10-16 20:47:36.516+00
428	24	13	2020-10-16 20:47:36.516+00
429	25	13	2020-10-16 20:47:36.516+00
430	26	13	2020-10-16 20:47:36.516+00
431	27	13	2020-10-16 20:47:36.516+00
432	28	13	2020-10-16 20:47:36.516+00
433	29	13	2020-10-16 20:47:36.516+00
1031	27	13	2021-01-28 19:08:04.044+00
1032	28	13	2021-01-28 19:08:04.044+00
438	99	50	2020-10-16 20:47:37.022+00
439	101	50	2020-10-16 20:47:37.022+00
440	102	50	2020-10-16 20:47:37.022+00
441	103	50	2020-10-16 20:47:37.022+00
442	23	13	2020-10-16 20:47:42.228+00
443	24	13	2020-10-16 20:47:42.228+00
444	25	13	2020-10-16 20:47:42.228+00
445	26	13	2020-10-16 20:47:42.228+00
446	27	13	2020-10-16 20:47:42.228+00
447	28	13	2020-10-16 20:47:42.228+00
448	29	13	2020-10-16 20:47:42.228+00
453	99	50	2020-10-16 20:47:42.449+00
454	101	50	2020-10-16 20:47:42.449+00
455	102	50	2020-10-16 20:47:42.449+00
456	103	50	2020-10-16 20:47:42.449+00
457	23	13	2020-10-16 20:47:42.758+00
458	24	13	2020-10-16 20:47:42.758+00
459	25	13	2020-10-16 20:47:42.758+00
460	26	13	2020-10-16 20:47:42.758+00
461	27	13	2020-10-16 20:47:42.758+00
462	28	13	2020-10-16 20:47:42.758+00
463	29	13	2020-10-16 20:47:42.758+00
468	99	50	2020-10-16 20:47:43.013+00
469	101	50	2020-10-16 20:47:43.013+00
470	102	50	2020-10-16 20:47:43.013+00
471	103	50	2020-10-16 20:47:43.013+00
472	23	13	2020-10-17 02:05:17.406+00
473	24	13	2020-10-17 02:05:17.406+00
474	25	13	2020-10-17 02:05:17.406+00
475	26	13	2020-10-17 02:05:17.406+00
476	27	13	2020-10-17 02:05:17.406+00
477	28	13	2020-10-17 02:05:17.406+00
478	29	13	2020-10-17 02:05:17.406+00
483	99	50	2020-10-17 02:05:18.156+00
484	101	50	2020-10-17 02:05:18.156+00
485	102	50	2020-10-17 02:05:18.156+00
486	103	50	2020-10-17 02:05:18.156+00
487	23	13	2020-10-17 02:05:26.345+00
488	24	13	2020-10-17 02:05:26.345+00
489	25	13	2020-10-17 02:05:26.345+00
490	26	13	2020-10-17 02:05:26.345+00
491	27	13	2020-10-17 02:05:26.345+00
492	28	13	2020-10-17 02:05:26.345+00
493	29	13	2020-10-17 02:05:26.345+00
498	99	50	2020-10-17 02:05:26.683+00
499	101	50	2020-10-17 02:05:26.683+00
500	102	50	2020-10-17 02:05:26.683+00
501	103	50	2020-10-17 02:05:26.683+00
502	23	13	2020-10-17 02:05:27.075+00
503	24	13	2020-10-17 02:05:27.075+00
504	25	13	2020-10-17 02:05:27.075+00
505	26	13	2020-10-17 02:05:27.075+00
506	27	13	2020-10-17 02:05:27.075+00
507	28	13	2020-10-17 02:05:27.075+00
508	29	13	2020-10-17 02:05:27.075+00
513	99	50	2020-10-17 02:05:27.428+00
514	101	50	2020-10-17 02:05:27.428+00
515	102	50	2020-10-17 02:05:27.428+00
516	103	50	2020-10-17 02:05:27.428+00
517	23	13	2020-11-03 14:57:04.152+00
518	24	13	2020-11-03 14:57:04.152+00
519	25	13	2020-11-03 14:57:04.152+00
520	26	13	2020-11-03 14:57:04.152+00
521	27	13	2020-11-03 14:57:04.152+00
522	28	13	2020-11-03 14:57:04.152+00
523	29	13	2020-11-03 14:57:04.152+00
528	99	50	2020-11-03 14:57:05.084+00
529	101	50	2020-11-03 14:57:05.084+00
530	102	50	2020-11-03 14:57:05.084+00
531	103	50	2020-11-03 14:57:05.084+00
532	23	13	2020-11-03 14:57:11.693+00
533	24	13	2020-11-03 14:57:11.693+00
534	25	13	2020-11-03 14:57:11.693+00
535	26	13	2020-11-03 14:57:11.693+00
536	27	13	2020-11-03 14:57:11.693+00
537	28	13	2020-11-03 14:57:11.693+00
538	29	13	2020-11-03 14:57:11.693+00
543	99	50	2020-11-03 14:57:11.91+00
544	101	50	2020-11-03 14:57:11.91+00
545	102	50	2020-11-03 14:57:11.91+00
546	103	50	2020-11-03 14:57:11.91+00
547	23	13	2020-11-03 14:57:12.16+00
548	24	13	2020-11-03 14:57:12.16+00
549	25	13	2020-11-03 14:57:12.16+00
550	26	13	2020-11-03 14:57:12.16+00
551	27	13	2020-11-03 14:57:12.16+00
552	28	13	2020-11-03 14:57:12.16+00
553	29	13	2020-11-03 14:57:12.16+00
558	99	50	2020-11-03 14:57:12.381+00
559	101	50	2020-11-03 14:57:12.381+00
560	102	50	2020-11-03 14:57:12.381+00
561	103	50	2020-11-03 14:57:12.381+00
562	23	13	2021-01-08 05:26:17.092+00
563	24	13	2021-01-08 05:26:17.092+00
564	25	13	2021-01-08 05:26:17.092+00
565	26	13	2021-01-08 05:26:17.092+00
566	27	13	2021-01-08 05:26:17.092+00
567	28	13	2021-01-08 05:26:17.092+00
568	29	13	2021-01-08 05:26:17.092+00
573	99	50	2021-01-08 05:26:17.953+00
574	101	50	2021-01-08 05:26:17.953+00
575	102	50	2021-01-08 05:26:17.953+00
576	103	50	2021-01-08 05:26:17.953+00
577	23	13	2021-01-08 05:26:24.844+00
578	24	13	2021-01-08 05:26:24.844+00
579	25	13	2021-01-08 05:26:24.844+00
580	26	13	2021-01-08 05:26:24.844+00
581	27	13	2021-01-08 05:26:24.844+00
582	28	13	2021-01-08 05:26:24.844+00
583	29	13	2021-01-08 05:26:24.844+00
588	99	50	2021-01-08 05:26:25.067+00
589	101	50	2021-01-08 05:26:25.067+00
590	102	50	2021-01-08 05:26:25.067+00
591	103	50	2021-01-08 05:26:25.067+00
592	23	13	2021-01-08 05:26:25.336+00
593	24	13	2021-01-08 05:26:25.336+00
594	25	13	2021-01-08 05:26:25.336+00
595	26	13	2021-01-08 05:26:25.336+00
596	27	13	2021-01-08 05:26:25.336+00
597	28	13	2021-01-08 05:26:25.336+00
598	29	13	2021-01-08 05:26:25.336+00
603	99	50	2021-01-08 05:26:25.565+00
604	101	50	2021-01-08 05:26:25.565+00
605	102	50	2021-01-08 05:26:25.565+00
606	103	50	2021-01-08 05:26:25.565+00
607	23	13	2021-01-08 05:56:49.597+00
608	24	13	2021-01-08 05:56:49.597+00
609	25	13	2021-01-08 05:56:49.597+00
610	26	13	2021-01-08 05:56:49.597+00
611	27	13	2021-01-08 05:56:49.597+00
612	28	13	2021-01-08 05:56:49.597+00
613	29	13	2021-01-08 05:56:49.597+00
618	99	50	2021-01-08 05:56:50.494+00
619	101	50	2021-01-08 05:56:50.494+00
620	102	50	2021-01-08 05:56:50.494+00
621	103	50	2021-01-08 05:56:50.494+00
622	23	13	2021-01-08 05:56:57.222+00
623	24	13	2021-01-08 05:56:57.222+00
624	25	13	2021-01-08 05:56:57.222+00
625	26	13	2021-01-08 05:56:57.222+00
626	27	13	2021-01-08 05:56:57.222+00
627	28	13	2021-01-08 05:56:57.222+00
628	29	13	2021-01-08 05:56:57.222+00
633	99	50	2021-01-08 05:56:57.438+00
634	101	50	2021-01-08 05:56:57.438+00
635	102	50	2021-01-08 05:56:57.438+00
636	103	50	2021-01-08 05:56:57.438+00
637	23	13	2021-01-08 05:56:57.718+00
638	24	13	2021-01-08 05:56:57.718+00
639	25	13	2021-01-08 05:56:57.718+00
640	26	13	2021-01-08 05:56:57.718+00
641	27	13	2021-01-08 05:56:57.718+00
642	28	13	2021-01-08 05:56:57.718+00
643	29	13	2021-01-08 05:56:57.718+00
648	99	50	2021-01-08 05:56:57.953+00
649	101	50	2021-01-08 05:56:57.953+00
650	102	50	2021-01-08 05:56:57.953+00
651	103	50	2021-01-08 05:56:57.953+00
652	23	13	2021-01-10 19:21:29.835+00
653	24	13	2021-01-10 19:21:29.835+00
654	25	13	2021-01-10 19:21:29.835+00
655	26	13	2021-01-10 19:21:29.835+00
656	27	13	2021-01-10 19:21:29.835+00
657	28	13	2021-01-10 19:21:29.835+00
658	29	13	2021-01-10 19:21:29.835+00
663	99	50	2021-01-10 19:21:30.74+00
664	101	50	2021-01-10 19:21:30.74+00
665	102	50	2021-01-10 19:21:30.74+00
666	103	50	2021-01-10 19:21:30.74+00
667	23	13	2021-01-10 19:21:37.636+00
668	24	13	2021-01-10 19:21:37.636+00
669	25	13	2021-01-10 19:21:37.636+00
670	26	13	2021-01-10 19:21:37.636+00
671	27	13	2021-01-10 19:21:37.636+00
672	28	13	2021-01-10 19:21:37.636+00
673	29	13	2021-01-10 19:21:37.636+00
678	99	50	2021-01-10 19:21:37.924+00
679	101	50	2021-01-10 19:21:37.924+00
680	102	50	2021-01-10 19:21:37.924+00
681	103	50	2021-01-10 19:21:37.924+00
682	23	13	2021-01-10 19:21:38.286+00
683	24	13	2021-01-10 19:21:38.286+00
684	25	13	2021-01-10 19:21:38.286+00
685	26	13	2021-01-10 19:21:38.286+00
686	27	13	2021-01-10 19:21:38.286+00
687	28	13	2021-01-10 19:21:38.286+00
688	29	13	2021-01-10 19:21:38.286+00
693	99	50	2021-01-10 19:21:38.634+00
694	101	50	2021-01-10 19:21:38.634+00
695	102	50	2021-01-10 19:21:38.634+00
696	103	50	2021-01-10 19:21:38.634+00
697	23	13	2021-01-10 20:58:16.833+00
698	24	13	2021-01-10 20:58:16.833+00
699	25	13	2021-01-10 20:58:16.833+00
700	26	13	2021-01-10 20:58:16.833+00
701	27	13	2021-01-10 20:58:16.833+00
702	28	13	2021-01-10 20:58:16.833+00
703	29	13	2021-01-10 20:58:16.833+00
708	99	50	2021-01-10 20:58:17.546+00
709	101	50	2021-01-10 20:58:17.546+00
710	102	50	2021-01-10 20:58:17.546+00
711	103	50	2021-01-10 20:58:17.546+00
712	23	13	2021-01-10 20:58:23.659+00
713	24	13	2021-01-10 20:58:23.659+00
714	25	13	2021-01-10 20:58:23.659+00
715	26	13	2021-01-10 20:58:23.659+00
716	27	13	2021-01-10 20:58:23.659+00
717	28	13	2021-01-10 20:58:23.659+00
718	29	13	2021-01-10 20:58:23.659+00
723	99	50	2021-01-10 20:58:23.871+00
724	101	50	2021-01-10 20:58:23.871+00
725	102	50	2021-01-10 20:58:23.871+00
726	103	50	2021-01-10 20:58:23.871+00
727	23	13	2021-01-10 20:58:24.131+00
728	24	13	2021-01-10 20:58:24.131+00
729	25	13	2021-01-10 20:58:24.131+00
730	26	13	2021-01-10 20:58:24.131+00
731	27	13	2021-01-10 20:58:24.131+00
732	28	13	2021-01-10 20:58:24.131+00
733	29	13	2021-01-10 20:58:24.131+00
738	99	50	2021-01-10 20:58:24.367+00
739	101	50	2021-01-10 20:58:24.367+00
740	102	50	2021-01-10 20:58:24.367+00
741	103	50	2021-01-10 20:58:24.367+00
742	23	13	2021-01-12 16:45:39.69+00
743	24	13	2021-01-12 16:45:39.69+00
744	25	13	2021-01-12 16:45:39.69+00
745	26	13	2021-01-12 16:45:39.69+00
746	27	13	2021-01-12 16:45:39.69+00
747	28	13	2021-01-12 16:45:39.69+00
748	29	13	2021-01-12 16:45:39.69+00
753	99	50	2021-01-12 16:45:41.018+00
754	101	50	2021-01-12 16:45:41.018+00
755	102	50	2021-01-12 16:45:41.018+00
756	103	50	2021-01-12 16:45:41.018+00
757	23	13	2021-01-12 16:45:48.066+00
758	24	13	2021-01-12 16:45:48.066+00
759	25	13	2021-01-12 16:45:48.066+00
760	26	13	2021-01-12 16:45:48.066+00
761	27	13	2021-01-12 16:45:48.066+00
762	28	13	2021-01-12 16:45:48.066+00
763	29	13	2021-01-12 16:45:48.066+00
768	99	50	2021-01-12 16:45:48.376+00
769	101	50	2021-01-12 16:45:48.376+00
770	102	50	2021-01-12 16:45:48.376+00
771	103	50	2021-01-12 16:45:48.376+00
772	23	13	2021-01-12 16:45:48.682+00
773	24	13	2021-01-12 16:45:48.682+00
774	25	13	2021-01-12 16:45:48.682+00
775	26	13	2021-01-12 16:45:48.682+00
776	27	13	2021-01-12 16:45:48.682+00
777	28	13	2021-01-12 16:45:48.682+00
778	29	13	2021-01-12 16:45:48.682+00
783	99	50	2021-01-12 16:45:49.018+00
784	101	50	2021-01-12 16:45:49.018+00
785	102	50	2021-01-12 16:45:49.018+00
786	103	50	2021-01-12 16:45:49.018+00
787	23	13	2021-01-13 19:21:57.67+00
788	24	13	2021-01-13 19:21:57.67+00
789	25	13	2021-01-13 19:21:57.67+00
790	26	13	2021-01-13 19:21:57.67+00
791	27	13	2021-01-13 19:21:57.67+00
792	28	13	2021-01-13 19:21:57.67+00
793	29	13	2021-01-13 19:21:57.67+00
798	99	50	2021-01-13 19:21:58.182+00
799	101	50	2021-01-13 19:21:58.182+00
800	102	50	2021-01-13 19:21:58.182+00
801	103	50	2021-01-13 19:21:58.182+00
802	23	13	2021-01-13 19:22:02.417+00
803	24	13	2021-01-13 19:22:02.417+00
804	25	13	2021-01-13 19:22:02.417+00
805	26	13	2021-01-13 19:22:02.417+00
806	27	13	2021-01-13 19:22:02.417+00
807	28	13	2021-01-13 19:22:02.417+00
808	29	13	2021-01-13 19:22:02.417+00
813	99	50	2021-01-13 19:22:02.664+00
814	101	50	2021-01-13 19:22:02.664+00
815	102	50	2021-01-13 19:22:02.664+00
816	103	50	2021-01-13 19:22:02.664+00
817	23	13	2021-01-13 19:22:03.029+00
818	24	13	2021-01-13 19:22:03.029+00
819	25	13	2021-01-13 19:22:03.029+00
820	26	13	2021-01-13 19:22:03.029+00
821	27	13	2021-01-13 19:22:03.029+00
822	28	13	2021-01-13 19:22:03.029+00
823	29	13	2021-01-13 19:22:03.029+00
828	99	50	2021-01-13 19:22:03.311+00
829	101	50	2021-01-13 19:22:03.311+00
830	102	50	2021-01-13 19:22:03.311+00
831	103	50	2021-01-13 19:22:03.311+00
832	23	13	2021-01-14 04:28:00.506+00
833	24	13	2021-01-14 04:28:00.506+00
834	25	13	2021-01-14 04:28:00.506+00
835	26	13	2021-01-14 04:28:00.506+00
836	27	13	2021-01-14 04:28:00.506+00
837	28	13	2021-01-14 04:28:00.506+00
838	29	13	2021-01-14 04:28:00.506+00
1033	29	13	2021-01-28 19:08:04.044+00
843	99	50	2021-01-14 04:28:01.235+00
844	101	50	2021-01-14 04:28:01.235+00
845	102	50	2021-01-14 04:28:01.235+00
846	103	50	2021-01-14 04:28:01.235+00
847	23	13	2021-01-14 04:28:07.928+00
848	24	13	2021-01-14 04:28:07.928+00
849	25	13	2021-01-14 04:28:07.928+00
850	26	13	2021-01-14 04:28:07.928+00
851	27	13	2021-01-14 04:28:07.928+00
852	28	13	2021-01-14 04:28:07.928+00
853	29	13	2021-01-14 04:28:07.928+00
1038	99	50	2021-01-28 19:08:05.852+00
1039	101	50	2021-01-28 19:08:05.852+00
1040	102	50	2021-01-28 19:08:05.852+00
858	99	50	2021-01-14 04:28:08.278+00
859	101	50	2021-01-14 04:28:08.278+00
860	102	50	2021-01-14 04:28:08.278+00
861	103	50	2021-01-14 04:28:08.278+00
862	23	13	2021-01-14 04:28:08.672+00
863	24	13	2021-01-14 04:28:08.672+00
864	25	13	2021-01-14 04:28:08.672+00
865	26	13	2021-01-14 04:28:08.672+00
866	27	13	2021-01-14 04:28:08.672+00
867	28	13	2021-01-14 04:28:08.672+00
868	29	13	2021-01-14 04:28:08.672+00
1041	103	50	2021-01-28 19:08:05.852+00
1042	23	13	2021-01-28 19:08:07.392+00
1043	24	13	2021-01-28 19:08:07.392+00
1044	25	13	2021-01-28 19:08:07.392+00
873	99	50	2021-01-14 04:28:09.052+00
874	101	50	2021-01-14 04:28:09.052+00
875	102	50	2021-01-14 04:28:09.052+00
876	103	50	2021-01-14 04:28:09.052+00
877	23	13	2021-01-21 18:55:59.007+00
878	24	13	2021-01-21 18:55:59.007+00
879	25	13	2021-01-21 18:55:59.007+00
880	26	13	2021-01-21 18:55:59.007+00
881	27	13	2021-01-21 18:55:59.007+00
882	28	13	2021-01-21 18:55:59.007+00
883	29	13	2021-01-21 18:55:59.007+00
1045	26	13	2021-01-28 19:08:07.392+00
1046	27	13	2021-01-28 19:08:07.392+00
1047	28	13	2021-01-28 19:08:07.392+00
1048	29	13	2021-01-28 19:08:07.392+00
888	99	50	2021-01-21 18:56:01.305+00
889	101	50	2021-01-21 18:56:01.305+00
890	102	50	2021-01-21 18:56:01.305+00
891	103	50	2021-01-21 18:56:01.306+00
892	23	13	2021-01-21 18:56:41.575+00
893	24	13	2021-01-21 18:56:41.575+00
894	25	13	2021-01-21 18:56:41.575+00
895	26	13	2021-01-21 18:56:41.575+00
896	27	13	2021-01-21 18:56:41.575+00
897	28	13	2021-01-21 18:56:41.575+00
898	29	13	2021-01-21 18:56:41.575+00
903	99	50	2021-01-21 18:56:43.381+00
904	101	50	2021-01-21 18:56:43.381+00
905	102	50	2021-01-21 18:56:43.381+00
906	103	50	2021-01-21 18:56:43.381+00
907	23	13	2021-01-21 18:56:45.07+00
908	24	13	2021-01-21 18:56:45.07+00
909	25	13	2021-01-21 18:56:45.07+00
910	26	13	2021-01-21 18:56:45.07+00
911	27	13	2021-01-21 18:56:45.07+00
912	28	13	2021-01-21 18:56:45.07+00
913	29	13	2021-01-21 18:56:45.07+00
918	99	50	2021-01-21 18:56:46.841+00
919	101	50	2021-01-21 18:56:46.841+00
920	102	50	2021-01-21 18:56:46.841+00
921	103	50	2021-01-21 18:56:46.841+00
922	23	13	2021-01-22 20:34:40.554+00
923	24	13	2021-01-22 20:34:40.554+00
924	25	13	2021-01-22 20:34:40.554+00
925	26	13	2021-01-22 20:34:40.554+00
926	27	13	2021-01-22 20:34:40.554+00
927	28	13	2021-01-22 20:34:40.554+00
928	29	13	2021-01-22 20:34:40.554+00
933	99	50	2021-01-22 20:34:41.221+00
934	101	50	2021-01-22 20:34:41.221+00
935	102	50	2021-01-22 20:34:41.221+00
936	103	50	2021-01-22 20:34:41.221+00
937	23	13	2021-01-22 20:34:49.294+00
938	24	13	2021-01-22 20:34:49.294+00
939	25	13	2021-01-22 20:34:49.294+00
940	26	13	2021-01-22 20:34:49.294+00
941	27	13	2021-01-22 20:34:49.294+00
942	28	13	2021-01-22 20:34:49.294+00
943	29	13	2021-01-22 20:34:49.294+00
948	99	50	2021-01-22 20:34:49.727+00
949	101	50	2021-01-22 20:34:49.727+00
950	102	50	2021-01-22 20:34:49.727+00
951	103	50	2021-01-22 20:34:49.727+00
952	23	13	2021-01-22 20:34:50.217+00
953	24	13	2021-01-22 20:34:50.217+00
954	25	13	2021-01-22 20:34:50.217+00
955	26	13	2021-01-22 20:34:50.217+00
956	27	13	2021-01-22 20:34:50.217+00
957	28	13	2021-01-22 20:34:50.217+00
958	29	13	2021-01-22 20:34:50.217+00
963	99	50	2021-01-22 20:34:50.662+00
964	101	50	2021-01-22 20:34:50.662+00
965	102	50	2021-01-22 20:34:50.662+00
966	103	50	2021-01-22 20:34:50.662+00
967	23	13	2021-01-25 15:12:17.204+00
968	24	13	2021-01-25 15:12:17.204+00
969	25	13	2021-01-25 15:12:17.204+00
970	26	13	2021-01-25 15:12:17.204+00
971	27	13	2021-01-25 15:12:17.204+00
972	28	13	2021-01-25 15:12:17.204+00
973	29	13	2021-01-25 15:12:17.204+00
978	99	50	2021-01-25 15:12:17.533+00
979	101	50	2021-01-25 15:12:17.534+00
980	102	50	2021-01-25 15:12:17.534+00
981	103	50	2021-01-25 15:12:17.534+00
982	23	13	2021-01-25 15:12:21.264+00
983	24	13	2021-01-25 15:12:21.264+00
984	25	13	2021-01-25 15:12:21.264+00
985	26	13	2021-01-25 15:12:21.264+00
986	27	13	2021-01-25 15:12:21.264+00
987	28	13	2021-01-25 15:12:21.264+00
988	29	13	2021-01-25 15:12:21.264+00
993	99	50	2021-01-25 15:12:21.461+00
994	101	50	2021-01-25 15:12:21.461+00
995	102	50	2021-01-25 15:12:21.461+00
996	103	50	2021-01-25 15:12:21.461+00
997	23	13	2021-01-25 15:12:21.695+00
998	24	13	2021-01-25 15:12:21.695+00
999	25	13	2021-01-25 15:12:21.695+00
1000	26	13	2021-01-25 15:12:21.695+00
1001	27	13	2021-01-25 15:12:21.695+00
1002	28	13	2021-01-25 15:12:21.695+00
1003	29	13	2021-01-25 15:12:21.695+00
1008	99	50	2021-01-25 15:12:21.894+00
1009	101	50	2021-01-25 15:12:21.894+00
1010	102	50	2021-01-25 15:12:21.894+00
1011	103	50	2021-01-25 15:12:21.894+00
1012	23	13	2021-01-28 19:07:22.778+00
1013	24	13	2021-01-28 19:07:22.778+00
1014	25	13	2021-01-28 19:07:22.778+00
1015	26	13	2021-01-28 19:07:22.778+00
1016	27	13	2021-01-28 19:07:22.778+00
1017	28	13	2021-01-28 19:07:22.778+00
1018	29	13	2021-01-28 19:07:22.778+00
1023	99	50	2021-01-28 19:07:24.805+00
1024	101	50	2021-01-28 19:07:24.805+00
1025	102	50	2021-01-28 19:07:24.805+00
1026	103	50	2021-01-28 19:07:24.805+00
1053	99	50	2021-01-28 19:08:09.071+00
1054	101	50	2021-01-28 19:08:09.071+00
1055	102	50	2021-01-28 19:08:09.071+00
1056	103	50	2021-01-28 19:08:09.071+00
1057	23	13	2021-02-05 04:40:32.098+00
1058	24	13	2021-02-05 04:40:32.098+00
1059	25	13	2021-02-05 04:40:32.098+00
1060	26	13	2021-02-05 04:40:32.098+00
1061	27	13	2021-02-05 04:40:32.098+00
1062	28	13	2021-02-05 04:40:32.098+00
1063	29	13	2021-02-05 04:40:32.098+00
1068	99	50	2021-02-05 04:40:34.231+00
1069	101	50	2021-02-05 04:40:34.232+00
1070	102	50	2021-02-05 04:40:34.232+00
1071	103	50	2021-02-05 04:40:34.232+00
1072	23	13	2021-02-05 04:41:16.665+00
1073	24	13	2021-02-05 04:41:16.665+00
1074	25	13	2021-02-05 04:41:16.665+00
1075	26	13	2021-02-05 04:41:16.665+00
1076	27	13	2021-02-05 04:41:16.665+00
1077	28	13	2021-02-05 04:41:16.665+00
1078	29	13	2021-02-05 04:41:16.665+00
1083	99	50	2021-02-05 04:41:18.492+00
1084	101	50	2021-02-05 04:41:18.492+00
1085	102	50	2021-02-05 04:41:18.492+00
1086	103	50	2021-02-05 04:41:18.492+00
1087	23	13	2021-02-05 04:41:20.152+00
1088	24	13	2021-02-05 04:41:20.152+00
1089	25	13	2021-02-05 04:41:20.152+00
1090	26	13	2021-02-05 04:41:20.152+00
1091	27	13	2021-02-05 04:41:20.152+00
1092	28	13	2021-02-05 04:41:20.152+00
1093	29	13	2021-02-05 04:41:20.152+00
1098	99	50	2021-02-05 04:41:21.998+00
1099	101	50	2021-02-05 04:41:21.998+00
1100	102	50	2021-02-05 04:41:21.998+00
1101	103	50	2021-02-05 04:41:21.998+00
1102	23	13	2021-02-06 04:44:40.416+00
1103	24	13	2021-02-06 04:44:40.416+00
1104	25	13	2021-02-06 04:44:40.416+00
1105	26	13	2021-02-06 04:44:40.416+00
1106	27	13	2021-02-06 04:44:40.416+00
1107	28	13	2021-02-06 04:44:40.416+00
1108	29	13	2021-02-06 04:44:40.416+00
1113	99	50	2021-02-06 04:44:42.543+00
1114	101	50	2021-02-06 04:44:42.543+00
1115	102	50	2021-02-06 04:44:42.543+00
1116	103	50	2021-02-06 04:44:42.543+00
1117	23	13	2021-02-06 04:45:24.621+00
1118	24	13	2021-02-06 04:45:24.621+00
1119	25	13	2021-02-06 04:45:24.621+00
1120	26	13	2021-02-06 04:45:24.621+00
1121	27	13	2021-02-06 04:45:24.621+00
1122	28	13	2021-02-06 04:45:24.621+00
1123	29	13	2021-02-06 04:45:24.621+00
1128	99	50	2021-02-06 04:45:26.535+00
1129	101	50	2021-02-06 04:45:26.535+00
1130	102	50	2021-02-06 04:45:26.535+00
1131	103	50	2021-02-06 04:45:26.535+00
1132	23	13	2021-02-06 04:45:28.294+00
1133	24	13	2021-02-06 04:45:28.294+00
1134	25	13	2021-02-06 04:45:28.294+00
1135	26	13	2021-02-06 04:45:28.294+00
1136	27	13	2021-02-06 04:45:28.294+00
1137	28	13	2021-02-06 04:45:28.294+00
1138	29	13	2021-02-06 04:45:28.294+00
1143	99	50	2021-02-06 04:45:30.201+00
1144	101	50	2021-02-06 04:45:30.201+00
1145	102	50	2021-02-06 04:45:30.201+00
1146	103	50	2021-02-06 04:45:30.201+00
1147	23	705	2021-02-09 15:48:41.373+00
1148	24	705	2021-02-09 15:48:41.373+00
1149	25	705	2021-02-09 15:48:41.373+00
1150	26	705	2021-02-09 15:48:41.373+00
1151	27	705	2021-02-09 15:48:41.373+00
1152	28	705	2021-02-09 15:48:41.373+00
1153	29	705	2021-02-09 15:48:41.373+00
1158	99	706	2021-02-09 15:48:43.637+00
1159	101	706	2021-02-09 15:48:43.637+00
1160	102	706	2021-02-09 15:48:43.637+00
1161	103	706	2021-02-09 15:48:43.637+00
1162	23	705	2021-02-09 15:49:28.839+00
1163	24	705	2021-02-09 15:49:28.839+00
1164	25	705	2021-02-09 15:49:28.839+00
1165	26	705	2021-02-09 15:49:28.839+00
1166	27	705	2021-02-09 15:49:28.839+00
1167	28	705	2021-02-09 15:49:28.839+00
1168	29	705	2021-02-09 15:49:28.839+00
1173	99	706	2021-02-09 15:49:30.797+00
1174	101	706	2021-02-09 15:49:30.797+00
1175	102	706	2021-02-09 15:49:30.797+00
1176	103	706	2021-02-09 15:49:30.797+00
1177	23	705	2021-02-09 15:49:32.583+00
1178	24	705	2021-02-09 15:49:32.583+00
1179	25	705	2021-02-09 15:49:32.583+00
1180	26	705	2021-02-09 15:49:32.583+00
1181	27	705	2021-02-09 15:49:32.583+00
1182	28	705	2021-02-09 15:49:32.583+00
1183	29	705	2021-02-09 15:49:32.583+00
1188	99	706	2021-02-09 15:49:34.542+00
1189	101	706	2021-02-09 15:49:34.542+00
1190	102	706	2021-02-09 15:49:34.542+00
1191	103	706	2021-02-09 15:49:34.542+00
1192	23	705	2021-02-09 15:51:13.315+00
1193	24	705	2021-02-09 15:51:13.315+00
1194	25	705	2021-02-09 15:51:13.315+00
1195	26	705	2021-02-09 15:51:13.315+00
1196	27	705	2021-02-09 15:51:13.315+00
1197	28	705	2021-02-09 15:51:13.315+00
1198	29	705	2021-02-09 15:51:13.315+00
1203	99	706	2021-02-09 15:51:15.516+00
1204	101	706	2021-02-09 15:51:15.516+00
1205	102	706	2021-02-09 15:51:15.516+00
1206	103	706	2021-02-09 15:51:15.516+00
1207	23	705	2021-02-09 15:52:01.274+00
1208	24	705	2021-02-09 15:52:01.274+00
1209	25	705	2021-02-09 15:52:01.274+00
1210	26	705	2021-02-09 15:52:01.274+00
1211	27	705	2021-02-09 15:52:01.274+00
1212	28	705	2021-02-09 15:52:01.274+00
1213	29	705	2021-02-09 15:52:01.274+00
1218	99	706	2021-02-09 15:52:03.233+00
1219	101	706	2021-02-09 15:52:03.233+00
1220	102	706	2021-02-09 15:52:03.233+00
1221	103	706	2021-02-09 15:52:03.233+00
1222	23	705	2021-02-09 15:52:04.99+00
1223	24	705	2021-02-09 15:52:04.99+00
1224	25	705	2021-02-09 15:52:04.99+00
1225	26	705	2021-02-09 15:52:04.99+00
1226	27	705	2021-02-09 15:52:04.99+00
1227	28	705	2021-02-09 15:52:04.99+00
1228	29	705	2021-02-09 15:52:04.99+00
1233	99	706	2021-02-09 15:52:06.959+00
1234	101	706	2021-02-09 15:52:06.959+00
1235	102	706	2021-02-09 15:52:06.959+00
1236	103	706	2021-02-09 15:52:06.959+00
1237	23	705	2021-02-09 15:53:51.555+00
1238	24	705	2021-02-09 15:53:51.555+00
1239	25	705	2021-02-09 15:53:51.555+00
1240	26	705	2021-02-09 15:53:51.555+00
1241	27	705	2021-02-09 15:53:51.555+00
1242	28	705	2021-02-09 15:53:51.555+00
1243	29	705	2021-02-09 15:53:51.555+00
1248	99	706	2021-02-09 15:53:53.813+00
1249	101	706	2021-02-09 15:53:53.813+00
1250	102	706	2021-02-09 15:53:53.813+00
1251	103	706	2021-02-09 15:53:53.813+00
1252	23	705	2021-02-09 15:54:39.556+00
1253	24	705	2021-02-09 15:54:39.556+00
1254	25	705	2021-02-09 15:54:39.556+00
1255	26	705	2021-02-09 15:54:39.556+00
1256	27	705	2021-02-09 15:54:39.556+00
1257	28	705	2021-02-09 15:54:39.556+00
1258	29	705	2021-02-09 15:54:39.556+00
1263	99	706	2021-02-09 15:54:41.549+00
1264	101	706	2021-02-09 15:54:41.549+00
1265	102	706	2021-02-09 15:54:41.549+00
1266	103	706	2021-02-09 15:54:41.549+00
1267	23	705	2021-02-09 15:54:43.33+00
1268	24	705	2021-02-09 15:54:43.33+00
1269	25	705	2021-02-09 15:54:43.33+00
1270	26	705	2021-02-09 15:54:43.33+00
1271	27	705	2021-02-09 15:54:43.33+00
1272	28	705	2021-02-09 15:54:43.33+00
1273	29	705	2021-02-09 15:54:43.33+00
1278	99	706	2021-02-09 15:54:45.307+00
1279	101	706	2021-02-09 15:54:45.307+00
1280	102	706	2021-02-09 15:54:45.307+00
1281	103	706	2021-02-09 15:54:45.307+00
1282	23	705	2021-02-09 15:56:22.286+00
1283	24	705	2021-02-09 15:56:22.286+00
1284	25	705	2021-02-09 15:56:22.286+00
1285	26	705	2021-02-09 15:56:22.286+00
1286	27	705	2021-02-09 15:56:22.286+00
1287	28	705	2021-02-09 15:56:22.286+00
1288	29	705	2021-02-09 15:56:22.286+00
1293	99	706	2021-02-09 15:56:24.547+00
1294	101	706	2021-02-09 15:56:24.547+00
1295	102	706	2021-02-09 15:56:24.547+00
1296	103	706	2021-02-09 15:56:24.547+00
1297	23	705	2021-02-09 15:57:10.099+00
1298	24	705	2021-02-09 15:57:10.099+00
1299	25	705	2021-02-09 15:57:10.099+00
1300	26	705	2021-02-09 15:57:10.099+00
1301	27	705	2021-02-09 15:57:10.099+00
1302	28	705	2021-02-09 15:57:10.099+00
1303	29	705	2021-02-09 15:57:10.099+00
1308	99	706	2021-02-09 15:57:12.059+00
1309	101	706	2021-02-09 15:57:12.059+00
1310	102	706	2021-02-09 15:57:12.059+00
1311	103	706	2021-02-09 15:57:12.059+00
1312	23	705	2021-02-09 15:57:13.807+00
1313	24	705	2021-02-09 15:57:13.807+00
1314	25	705	2021-02-09 15:57:13.807+00
1315	26	705	2021-02-09 15:57:13.807+00
1316	27	705	2021-02-09 15:57:13.807+00
1317	28	705	2021-02-09 15:57:13.807+00
1318	29	705	2021-02-09 15:57:13.807+00
1323	99	706	2021-02-09 15:57:15.749+00
1324	101	706	2021-02-09 15:57:15.749+00
1325	102	706	2021-02-09 15:57:15.749+00
1326	103	706	2021-02-09 15:57:15.749+00
1327	23	705	2021-02-09 16:05:51.283+00
1328	24	705	2021-02-09 16:05:51.283+00
1329	25	705	2021-02-09 16:05:51.283+00
1330	26	705	2021-02-09 16:05:51.283+00
1331	27	705	2021-02-09 16:05:51.283+00
1332	28	705	2021-02-09 16:05:51.283+00
1333	29	705	2021-02-09 16:05:51.283+00
1338	99	706	2021-02-09 16:05:52.31+00
1339	101	706	2021-02-09 16:05:52.31+00
1340	102	706	2021-02-09 16:05:52.31+00
1341	103	706	2021-02-09 16:05:52.31+00
1342	23	705	2021-02-09 16:06:10.149+00
1343	24	705	2021-02-09 16:06:10.149+00
1344	25	705	2021-02-09 16:06:10.149+00
1345	26	705	2021-02-09 16:06:10.149+00
1346	27	705	2021-02-09 16:06:10.149+00
1347	28	705	2021-02-09 16:06:10.149+00
1348	29	705	2021-02-09 16:06:10.149+00
1353	99	706	2021-02-09 16:06:10.924+00
1354	101	706	2021-02-09 16:06:10.924+00
1355	102	706	2021-02-09 16:06:10.924+00
1356	103	706	2021-02-09 16:06:10.924+00
1357	23	705	2021-02-09 16:06:11.634+00
1358	24	705	2021-02-09 16:06:11.634+00
1359	25	705	2021-02-09 16:06:11.634+00
1360	26	705	2021-02-09 16:06:11.634+00
1361	27	705	2021-02-09 16:06:11.634+00
1362	28	705	2021-02-09 16:06:11.634+00
1363	29	705	2021-02-09 16:06:11.634+00
1368	99	706	2021-02-09 16:06:12.398+00
1369	101	706	2021-02-09 16:06:12.398+00
1370	102	706	2021-02-09 16:06:12.398+00
1371	103	706	2021-02-09 16:06:12.398+00
1372	23	705	2021-02-09 16:07:05.292+00
1373	24	705	2021-02-09 16:07:05.292+00
1374	25	705	2021-02-09 16:07:05.292+00
1375	26	705	2021-02-09 16:07:05.292+00
1376	27	705	2021-02-09 16:07:05.292+00
1377	28	705	2021-02-09 16:07:05.292+00
1378	29	705	2021-02-09 16:07:05.292+00
1383	99	706	2021-02-09 16:07:06.373+00
1384	101	706	2021-02-09 16:07:06.373+00
1385	102	706	2021-02-09 16:07:06.373+00
1386	103	706	2021-02-09 16:07:06.373+00
1387	23	705	2021-02-09 16:07:24.143+00
1388	24	705	2021-02-09 16:07:24.143+00
1389	25	705	2021-02-09 16:07:24.143+00
1390	26	705	2021-02-09 16:07:24.143+00
1391	27	705	2021-02-09 16:07:24.143+00
1392	28	705	2021-02-09 16:07:24.143+00
1393	29	705	2021-02-09 16:07:24.143+00
1398	99	706	2021-02-09 16:07:24.887+00
1399	101	706	2021-02-09 16:07:24.887+00
1400	102	706	2021-02-09 16:07:24.887+00
1401	103	706	2021-02-09 16:07:24.887+00
1402	23	705	2021-02-09 16:07:25.576+00
1403	24	705	2021-02-09 16:07:25.576+00
1404	25	705	2021-02-09 16:07:25.576+00
1405	26	705	2021-02-09 16:07:25.576+00
1406	27	705	2021-02-09 16:07:25.576+00
1407	28	705	2021-02-09 16:07:25.576+00
1408	29	705	2021-02-09 16:07:25.576+00
1413	99	706	2021-02-09 16:07:26.317+00
1414	101	706	2021-02-09 16:07:26.317+00
1415	102	706	2021-02-09 16:07:26.317+00
1416	103	706	2021-02-09 16:07:26.317+00
1417	23	705	2021-02-09 16:08:24.412+00
1418	24	705	2021-02-09 16:08:24.412+00
1419	25	705	2021-02-09 16:08:24.412+00
1420	26	705	2021-02-09 16:08:24.412+00
1421	27	705	2021-02-09 16:08:24.412+00
1422	28	705	2021-02-09 16:08:24.412+00
1423	29	705	2021-02-09 16:08:24.412+00
1428	99	706	2021-02-09 16:08:25.516+00
1429	101	706	2021-02-09 16:08:25.516+00
1430	102	706	2021-02-09 16:08:25.516+00
1431	103	706	2021-02-09 16:08:25.516+00
1432	23	705	2021-02-09 16:08:42.783+00
1433	24	705	2021-02-09 16:08:42.783+00
1434	25	705	2021-02-09 16:08:42.783+00
1435	26	705	2021-02-09 16:08:42.783+00
1436	27	705	2021-02-09 16:08:42.783+00
1437	28	705	2021-02-09 16:08:42.783+00
1438	29	705	2021-02-09 16:08:42.783+00
1443	99	706	2021-02-09 16:08:43.537+00
1444	101	706	2021-02-09 16:08:43.537+00
1445	102	706	2021-02-09 16:08:43.537+00
1446	103	706	2021-02-09 16:08:43.537+00
1447	23	705	2021-02-09 16:08:44.239+00
1448	24	705	2021-02-09 16:08:44.239+00
1449	25	705	2021-02-09 16:08:44.239+00
1450	26	705	2021-02-09 16:08:44.239+00
1451	27	705	2021-02-09 16:08:44.239+00
1452	28	705	2021-02-09 16:08:44.239+00
1453	29	705	2021-02-09 16:08:44.239+00
1458	99	706	2021-02-09 16:08:44.991+00
1459	101	706	2021-02-09 16:08:44.991+00
1460	102	706	2021-02-09 16:08:44.991+00
1461	103	706	2021-02-09 16:08:44.991+00
1462	23	705	2021-02-09 16:09:37.368+00
1463	24	705	2021-02-09 16:09:37.368+00
1464	25	705	2021-02-09 16:09:37.368+00
1465	26	705	2021-02-09 16:09:37.368+00
1466	27	705	2021-02-09 16:09:37.368+00
1467	28	705	2021-02-09 16:09:37.368+00
1468	29	705	2021-02-09 16:09:37.368+00
1473	99	706	2021-02-09 16:09:38.461+00
1474	101	706	2021-02-09 16:09:38.461+00
1475	102	706	2021-02-09 16:09:38.461+00
1476	103	706	2021-02-09 16:09:38.461+00
1477	23	705	2021-02-09 16:09:56.665+00
1478	24	705	2021-02-09 16:09:56.665+00
1479	25	705	2021-02-09 16:09:56.665+00
1480	26	705	2021-02-09 16:09:56.665+00
1481	27	705	2021-02-09 16:09:56.665+00
1482	28	705	2021-02-09 16:09:56.665+00
1483	29	705	2021-02-09 16:09:56.665+00
1488	99	706	2021-02-09 16:09:57.428+00
1489	101	706	2021-02-09 16:09:57.428+00
1490	102	706	2021-02-09 16:09:57.428+00
1491	103	706	2021-02-09 16:09:57.428+00
1492	23	705	2021-02-09 16:09:58.134+00
1493	24	705	2021-02-09 16:09:58.134+00
1494	25	705	2021-02-09 16:09:58.134+00
1495	26	705	2021-02-09 16:09:58.134+00
1496	27	705	2021-02-09 16:09:58.134+00
1497	28	705	2021-02-09 16:09:58.134+00
1498	29	705	2021-02-09 16:09:58.134+00
1503	99	706	2021-02-09 16:09:58.905+00
1504	101	706	2021-02-09 16:09:58.905+00
1505	102	706	2021-02-09 16:09:58.905+00
1506	103	706	2021-02-09 16:09:58.905+00
1507	23	705	2021-02-09 19:44:03.121+00
1508	24	705	2021-02-09 19:44:03.121+00
1509	25	705	2021-02-09 19:44:03.121+00
1510	26	705	2021-02-09 19:44:03.121+00
1511	27	705	2021-02-09 19:44:03.121+00
1512	28	705	2021-02-09 19:44:03.121+00
1513	29	705	2021-02-09 19:44:03.121+00
1518	99	706	2021-02-09 19:44:03.925+00
1519	101	706	2021-02-09 19:44:03.925+00
1520	102	706	2021-02-09 19:44:03.925+00
1521	103	706	2021-02-09 19:44:03.925+00
1522	23	705	2021-02-09 19:44:18.325+00
1523	24	705	2021-02-09 19:44:18.325+00
1524	25	705	2021-02-09 19:44:18.325+00
1525	26	705	2021-02-09 19:44:18.325+00
1526	27	705	2021-02-09 19:44:18.325+00
1527	28	705	2021-02-09 19:44:18.325+00
1528	29	705	2021-02-09 19:44:18.325+00
1533	99	706	2021-02-09 19:44:18.994+00
1534	101	706	2021-02-09 19:44:18.994+00
1535	102	706	2021-02-09 19:44:18.994+00
1536	103	706	2021-02-09 19:44:18.994+00
1537	23	705	2021-02-09 19:44:19.6+00
1538	24	705	2021-02-09 19:44:19.6+00
1539	25	705	2021-02-09 19:44:19.6+00
1540	26	705	2021-02-09 19:44:19.6+00
1541	27	705	2021-02-09 19:44:19.6+00
1542	28	705	2021-02-09 19:44:19.6+00
1543	29	705	2021-02-09 19:44:19.6+00
1548	99	706	2021-02-09 19:44:20.25+00
1549	101	706	2021-02-09 19:44:20.25+00
1550	102	706	2021-02-09 19:44:20.25+00
1551	103	706	2021-02-09 19:44:20.25+00
1552	23	705	2021-02-09 19:45:12.883+00
1553	24	705	2021-02-09 19:45:12.883+00
1554	25	705	2021-02-09 19:45:12.883+00
1555	26	705	2021-02-09 19:45:12.883+00
1556	27	705	2021-02-09 19:45:12.883+00
1557	28	705	2021-02-09 19:45:12.883+00
1558	29	705	2021-02-09 19:45:12.883+00
1563	99	706	2021-02-09 19:45:13.808+00
1564	101	706	2021-02-09 19:45:13.808+00
1565	102	706	2021-02-09 19:45:13.808+00
1566	103	706	2021-02-09 19:45:13.808+00
1567	23	705	2021-02-09 19:45:29.267+00
1568	24	705	2021-02-09 19:45:29.267+00
1569	25	705	2021-02-09 19:45:29.267+00
1570	26	705	2021-02-09 19:45:29.267+00
1571	27	705	2021-02-09 19:45:29.267+00
1572	28	705	2021-02-09 19:45:29.267+00
1573	29	705	2021-02-09 19:45:29.267+00
1578	99	706	2021-02-09 19:45:30.018+00
1579	101	706	2021-02-09 19:45:30.018+00
1580	102	706	2021-02-09 19:45:30.018+00
1581	103	706	2021-02-09 19:45:30.018+00
1582	23	705	2021-02-09 19:45:30.626+00
1583	24	705	2021-02-09 19:45:30.626+00
1584	25	705	2021-02-09 19:45:30.626+00
1585	26	705	2021-02-09 19:45:30.626+00
1586	27	705	2021-02-09 19:45:30.626+00
1587	28	705	2021-02-09 19:45:30.626+00
1588	29	705	2021-02-09 19:45:30.626+00
1593	99	706	2021-02-09 19:45:31.253+00
1594	101	706	2021-02-09 19:45:31.253+00
1595	102	706	2021-02-09 19:45:31.253+00
1596	103	706	2021-02-09 19:45:31.253+00
1597	23	705	2021-02-09 19:46:28.657+00
1598	24	705	2021-02-09 19:46:28.657+00
1599	25	705	2021-02-09 19:46:28.657+00
1600	26	705	2021-02-09 19:46:28.657+00
1601	27	705	2021-02-09 19:46:28.657+00
1602	28	705	2021-02-09 19:46:28.657+00
1603	29	705	2021-02-09 19:46:28.657+00
1608	99	706	2021-02-09 19:46:29.621+00
1609	101	706	2021-02-09 19:46:29.621+00
1610	102	706	2021-02-09 19:46:29.621+00
1611	103	706	2021-02-09 19:46:29.621+00
1612	23	705	2021-02-09 19:46:44.814+00
1613	24	705	2021-02-09 19:46:44.814+00
1614	25	705	2021-02-09 19:46:44.814+00
1615	26	705	2021-02-09 19:46:44.814+00
1616	27	705	2021-02-09 19:46:44.814+00
1617	28	705	2021-02-09 19:46:44.814+00
1618	29	705	2021-02-09 19:46:44.814+00
1623	99	706	2021-02-09 19:46:45.51+00
1624	101	706	2021-02-09 19:46:45.51+00
1625	102	706	2021-02-09 19:46:45.51+00
1626	103	706	2021-02-09 19:46:45.51+00
1627	23	705	2021-02-09 19:46:46.137+00
1628	24	705	2021-02-09 19:46:46.137+00
1629	25	705	2021-02-09 19:46:46.137+00
1630	26	705	2021-02-09 19:46:46.137+00
1631	27	705	2021-02-09 19:46:46.137+00
1632	28	705	2021-02-09 19:46:46.137+00
1633	29	705	2021-02-09 19:46:46.137+00
1638	99	706	2021-02-09 19:46:46.779+00
1639	101	706	2021-02-09 19:46:46.779+00
1640	102	706	2021-02-09 19:46:46.779+00
1641	103	706	2021-02-09 19:46:46.779+00
1642	23	705	2021-02-09 19:47:38.698+00
1643	24	705	2021-02-09 19:47:38.698+00
1644	25	705	2021-02-09 19:47:38.698+00
1645	26	705	2021-02-09 19:47:38.698+00
1646	27	705	2021-02-09 19:47:38.698+00
1647	28	705	2021-02-09 19:47:38.698+00
1648	29	705	2021-02-09 19:47:38.698+00
1653	99	706	2021-02-09 19:47:39.619+00
1654	101	706	2021-02-09 19:47:39.619+00
1655	102	706	2021-02-09 19:47:39.619+00
1656	103	706	2021-02-09 19:47:39.619+00
1657	23	705	2021-02-09 19:47:54.599+00
1658	24	705	2021-02-09 19:47:54.599+00
1659	25	705	2021-02-09 19:47:54.599+00
1660	26	705	2021-02-09 19:47:54.599+00
1661	27	705	2021-02-09 19:47:54.599+00
1662	28	705	2021-02-09 19:47:54.599+00
1663	29	705	2021-02-09 19:47:54.599+00
1668	99	706	2021-02-09 19:47:55.239+00
1669	101	706	2021-02-09 19:47:55.239+00
1670	102	706	2021-02-09 19:47:55.239+00
1671	103	706	2021-02-09 19:47:55.239+00
1672	23	705	2021-02-09 19:47:55.841+00
1673	24	705	2021-02-09 19:47:55.841+00
1674	25	705	2021-02-09 19:47:55.841+00
1675	26	705	2021-02-09 19:47:55.841+00
1676	27	705	2021-02-09 19:47:55.841+00
1677	28	705	2021-02-09 19:47:55.841+00
1678	29	705	2021-02-09 19:47:55.841+00
1683	99	706	2021-02-09 19:47:56.498+00
1684	101	706	2021-02-09 19:47:56.498+00
1685	102	706	2021-02-09 19:47:56.498+00
1686	103	706	2021-02-09 19:47:56.498+00
1687	23	705	2021-02-09 20:50:55.753+00
1688	24	705	2021-02-09 20:50:55.753+00
1689	25	705	2021-02-09 20:50:55.753+00
1690	26	705	2021-02-09 20:50:55.753+00
1691	27	705	2021-02-09 20:50:55.753+00
1692	28	705	2021-02-09 20:50:55.753+00
1693	29	705	2021-02-09 20:50:55.753+00
1698	99	706	2021-02-09 20:50:56.903+00
1699	101	706	2021-02-09 20:50:56.903+00
1700	102	706	2021-02-09 20:50:56.903+00
1701	103	706	2021-02-09 20:50:56.903+00
1702	23	705	2021-02-09 20:51:12.723+00
1703	24	705	2021-02-09 20:51:12.723+00
1704	25	705	2021-02-09 20:51:12.723+00
1705	26	705	2021-02-09 20:51:12.723+00
1706	27	705	2021-02-09 20:51:12.723+00
1707	28	705	2021-02-09 20:51:12.723+00
1708	29	705	2021-02-09 20:51:12.723+00
1713	99	706	2021-02-09 20:51:13.418+00
1714	101	706	2021-02-09 20:51:13.418+00
1715	102	706	2021-02-09 20:51:13.418+00
1716	103	706	2021-02-09 20:51:13.418+00
1717	23	705	2021-02-09 20:51:14.054+00
1718	24	705	2021-02-09 20:51:14.054+00
1719	25	705	2021-02-09 20:51:14.054+00
1720	26	705	2021-02-09 20:51:14.054+00
1721	27	705	2021-02-09 20:51:14.054+00
1722	28	705	2021-02-09 20:51:14.054+00
1723	29	705	2021-02-09 20:51:14.054+00
1728	99	706	2021-02-09 20:51:14.75+00
1729	101	706	2021-02-09 20:51:14.75+00
1730	102	706	2021-02-09 20:51:14.75+00
1731	103	706	2021-02-09 20:51:14.75+00
1732	23	705	2021-02-09 20:51:59.205+00
1733	24	705	2021-02-09 20:51:59.205+00
1734	25	705	2021-02-09 20:51:59.205+00
1735	26	705	2021-02-09 20:51:59.205+00
1736	27	705	2021-02-09 20:51:59.205+00
1737	28	705	2021-02-09 20:51:59.205+00
1738	29	705	2021-02-09 20:51:59.205+00
1743	99	706	2021-02-09 20:52:00.138+00
1744	101	706	2021-02-09 20:52:00.138+00
1745	102	706	2021-02-09 20:52:00.138+00
1746	103	706	2021-02-09 20:52:00.138+00
1747	23	705	2021-02-09 20:52:16.494+00
1748	24	705	2021-02-09 20:52:16.494+00
1749	25	705	2021-02-09 20:52:16.494+00
1750	26	705	2021-02-09 20:52:16.494+00
1751	27	705	2021-02-09 20:52:16.494+00
1752	28	705	2021-02-09 20:52:16.494+00
1753	29	705	2021-02-09 20:52:16.494+00
1758	99	706	2021-02-09 20:52:17.173+00
1759	101	706	2021-02-09 20:52:17.173+00
1760	102	706	2021-02-09 20:52:17.173+00
1761	103	706	2021-02-09 20:52:17.173+00
1762	23	705	2021-02-09 20:52:17.789+00
1763	24	705	2021-02-09 20:52:17.789+00
1764	25	705	2021-02-09 20:52:17.789+00
1765	26	705	2021-02-09 20:52:17.789+00
1766	27	705	2021-02-09 20:52:17.789+00
1767	28	705	2021-02-09 20:52:17.789+00
1768	29	705	2021-02-09 20:52:17.789+00
1773	99	706	2021-02-09 20:52:18.453+00
1774	101	706	2021-02-09 20:52:18.453+00
1775	102	706	2021-02-09 20:52:18.453+00
1776	103	706	2021-02-09 20:52:18.453+00
1777	23	705	2021-02-09 20:53:08.598+00
1778	24	705	2021-02-09 20:53:08.598+00
1779	25	705	2021-02-09 20:53:08.598+00
1780	26	705	2021-02-09 20:53:08.598+00
1781	27	705	2021-02-09 20:53:08.598+00
1782	28	705	2021-02-09 20:53:08.598+00
1783	29	705	2021-02-09 20:53:08.598+00
1788	99	706	2021-02-09 20:53:09.596+00
1789	101	706	2021-02-09 20:53:09.596+00
1790	102	706	2021-02-09 20:53:09.596+00
1791	103	706	2021-02-09 20:53:09.596+00
1792	23	705	2021-02-09 20:53:25.545+00
1793	24	705	2021-02-09 20:53:25.545+00
1794	25	705	2021-02-09 20:53:25.545+00
1795	26	705	2021-02-09 20:53:25.545+00
1796	27	705	2021-02-09 20:53:25.545+00
1797	28	705	2021-02-09 20:53:25.545+00
1798	29	705	2021-02-09 20:53:25.545+00
1803	99	706	2021-02-09 20:53:26.22+00
1804	101	706	2021-02-09 20:53:26.22+00
1805	102	706	2021-02-09 20:53:26.22+00
1806	103	706	2021-02-09 20:53:26.22+00
1807	23	705	2021-02-09 20:53:26.851+00
1808	24	705	2021-02-09 20:53:26.851+00
1809	25	705	2021-02-09 20:53:26.851+00
1810	26	705	2021-02-09 20:53:26.851+00
1811	27	705	2021-02-09 20:53:26.851+00
1812	28	705	2021-02-09 20:53:26.851+00
1813	29	705	2021-02-09 20:53:26.851+00
1818	99	706	2021-02-09 20:53:27.545+00
1819	101	706	2021-02-09 20:53:27.545+00
1820	102	706	2021-02-09 20:53:27.545+00
1821	103	706	2021-02-09 20:53:27.545+00
1822	23	705	2021-02-09 20:54:13.475+00
1823	24	705	2021-02-09 20:54:13.475+00
1824	25	705	2021-02-09 20:54:13.475+00
1825	26	705	2021-02-09 20:54:13.475+00
1826	27	705	2021-02-09 20:54:13.475+00
1827	28	705	2021-02-09 20:54:13.475+00
1828	29	705	2021-02-09 20:54:13.475+00
1833	99	706	2021-02-09 20:54:14.455+00
1834	101	706	2021-02-09 20:54:14.455+00
1835	102	706	2021-02-09 20:54:14.455+00
1836	103	706	2021-02-09 20:54:14.455+00
1837	23	705	2021-02-09 20:54:30.475+00
1838	24	705	2021-02-09 20:54:30.475+00
1839	25	705	2021-02-09 20:54:30.475+00
1840	26	705	2021-02-09 20:54:30.475+00
1841	27	705	2021-02-09 20:54:30.475+00
1842	28	705	2021-02-09 20:54:30.475+00
1843	29	705	2021-02-09 20:54:30.475+00
1848	99	706	2021-02-09 20:54:31.153+00
1849	101	706	2021-02-09 20:54:31.153+00
1850	102	706	2021-02-09 20:54:31.153+00
1851	103	706	2021-02-09 20:54:31.153+00
1852	23	705	2021-02-09 20:54:31.772+00
1853	24	705	2021-02-09 20:54:31.772+00
1854	25	705	2021-02-09 20:54:31.772+00
1855	26	705	2021-02-09 20:54:31.772+00
1856	27	705	2021-02-09 20:54:31.772+00
1857	28	705	2021-02-09 20:54:31.772+00
1858	29	705	2021-02-09 20:54:31.772+00
1863	99	706	2021-02-09 20:54:32.439+00
1864	101	706	2021-02-09 20:54:32.439+00
1865	102	706	2021-02-09 20:54:32.439+00
1866	103	706	2021-02-09 20:54:32.439+00
1867	23	705	2021-02-09 21:35:04.107+00
1868	24	705	2021-02-09 21:35:04.107+00
1869	25	705	2021-02-09 21:35:04.107+00
1870	26	705	2021-02-09 21:35:04.107+00
1871	27	705	2021-02-09 21:35:04.107+00
1872	28	705	2021-02-09 21:35:04.107+00
1873	29	705	2021-02-09 21:35:04.107+00
1878	99	706	2021-02-09 21:35:04.651+00
1879	101	706	2021-02-09 21:35:04.651+00
1880	102	706	2021-02-09 21:35:04.651+00
1881	103	706	2021-02-09 21:35:04.651+00
1882	23	705	2021-02-09 21:35:09.774+00
1883	24	705	2021-02-09 21:35:09.774+00
1884	25	705	2021-02-09 21:35:09.774+00
1885	26	705	2021-02-09 21:35:09.774+00
1886	27	705	2021-02-09 21:35:09.774+00
1887	28	705	2021-02-09 21:35:09.774+00
1888	29	705	2021-02-09 21:35:09.774+00
1893	99	706	2021-02-09 21:35:09.947+00
1894	101	706	2021-02-09 21:35:09.947+00
1895	102	706	2021-02-09 21:35:09.947+00
1896	103	706	2021-02-09 21:35:09.947+00
1897	23	705	2021-02-09 21:35:10.178+00
1898	24	705	2021-02-09 21:35:10.178+00
1899	25	705	2021-02-09 21:35:10.178+00
1900	26	705	2021-02-09 21:35:10.178+00
1901	27	705	2021-02-09 21:35:10.178+00
1902	28	705	2021-02-09 21:35:10.178+00
1903	29	705	2021-02-09 21:35:10.178+00
1908	99	706	2021-02-09 21:35:10.377+00
1909	101	706	2021-02-09 21:35:10.377+00
1910	102	706	2021-02-09 21:35:10.377+00
1911	103	706	2021-02-09 21:35:10.377+00
1912	23	705	2021-02-09 21:35:39.747+00
1913	24	705	2021-02-09 21:35:39.747+00
1914	25	705	2021-02-09 21:35:39.747+00
1915	26	705	2021-02-09 21:35:39.747+00
1916	27	705	2021-02-09 21:35:39.747+00
1917	28	705	2021-02-09 21:35:39.747+00
1918	29	705	2021-02-09 21:35:39.747+00
1923	99	706	2021-02-09 21:35:40.222+00
1924	101	706	2021-02-09 21:35:40.222+00
1925	102	706	2021-02-09 21:35:40.222+00
1926	103	706	2021-02-09 21:35:40.222+00
1927	23	705	2021-02-09 21:35:45.372+00
1928	24	705	2021-02-09 21:35:45.372+00
1929	25	705	2021-02-09 21:35:45.372+00
1930	26	705	2021-02-09 21:35:45.372+00
1931	27	705	2021-02-09 21:35:45.372+00
1932	28	705	2021-02-09 21:35:45.372+00
1933	29	705	2021-02-09 21:35:45.372+00
1938	99	706	2021-02-09 21:35:45.551+00
1939	101	706	2021-02-09 21:35:45.551+00
1940	102	706	2021-02-09 21:35:45.551+00
1941	103	706	2021-02-09 21:35:45.551+00
1942	23	705	2021-02-09 21:35:45.767+00
1943	24	705	2021-02-09 21:35:45.767+00
1944	25	705	2021-02-09 21:35:45.767+00
1945	26	705	2021-02-09 21:35:45.767+00
1946	27	705	2021-02-09 21:35:45.767+00
1947	28	705	2021-02-09 21:35:45.767+00
1948	29	705	2021-02-09 21:35:45.767+00
1953	99	706	2021-02-09 21:35:45.973+00
1954	101	706	2021-02-09 21:35:45.973+00
1955	102	706	2021-02-09 21:35:45.973+00
1956	103	706	2021-02-09 21:35:45.973+00
1957	23	705	2021-02-09 21:36:22.999+00
1958	24	705	2021-02-09 21:36:22.999+00
1959	25	705	2021-02-09 21:36:22.999+00
1960	26	705	2021-02-09 21:36:22.999+00
1961	27	705	2021-02-09 21:36:22.999+00
1962	28	705	2021-02-09 21:36:22.999+00
1963	29	705	2021-02-09 21:36:22.999+00
1968	99	706	2021-02-09 21:36:23.527+00
1969	101	706	2021-02-09 21:36:23.527+00
1970	102	706	2021-02-09 21:36:23.527+00
1971	103	706	2021-02-09 21:36:23.527+00
1972	23	705	2021-02-09 21:36:28.554+00
1973	24	705	2021-02-09 21:36:28.554+00
1974	25	705	2021-02-09 21:36:28.554+00
1975	26	705	2021-02-09 21:36:28.554+00
1976	27	705	2021-02-09 21:36:28.554+00
1977	28	705	2021-02-09 21:36:28.554+00
1978	29	705	2021-02-09 21:36:28.554+00
1983	99	706	2021-02-09 21:36:28.754+00
1984	101	706	2021-02-09 21:36:28.754+00
1985	102	706	2021-02-09 21:36:28.754+00
1986	103	706	2021-02-09 21:36:28.754+00
1987	23	705	2021-02-09 21:36:28.99+00
1988	24	705	2021-02-09 21:36:28.99+00
1989	25	705	2021-02-09 21:36:28.99+00
1990	26	705	2021-02-09 21:36:28.99+00
1991	27	705	2021-02-09 21:36:28.99+00
1992	28	705	2021-02-09 21:36:28.99+00
1993	29	705	2021-02-09 21:36:28.99+00
1998	99	706	2021-02-09 21:36:29.202+00
1999	101	706	2021-02-09 21:36:29.202+00
2000	102	706	2021-02-09 21:36:29.202+00
2001	103	706	2021-02-09 21:36:29.202+00
2002	23	705	2021-02-09 21:36:58.306+00
2003	24	705	2021-02-09 21:36:58.306+00
2004	25	705	2021-02-09 21:36:58.306+00
2005	26	705	2021-02-09 21:36:58.306+00
2006	27	705	2021-02-09 21:36:58.306+00
2007	28	705	2021-02-09 21:36:58.306+00
2008	29	705	2021-02-09 21:36:58.306+00
2013	99	706	2021-02-09 21:36:58.825+00
2014	101	706	2021-02-09 21:36:58.825+00
2015	102	706	2021-02-09 21:36:58.825+00
2016	103	706	2021-02-09 21:36:58.825+00
2017	23	705	2021-02-09 21:37:03.921+00
2018	24	705	2021-02-09 21:37:03.921+00
2019	25	705	2021-02-09 21:37:03.921+00
2020	26	705	2021-02-09 21:37:03.921+00
2021	27	705	2021-02-09 21:37:03.921+00
2022	28	705	2021-02-09 21:37:03.921+00
2023	29	705	2021-02-09 21:37:03.921+00
2028	99	706	2021-02-09 21:37:04.094+00
2029	101	706	2021-02-09 21:37:04.094+00
2030	102	706	2021-02-09 21:37:04.094+00
2031	103	706	2021-02-09 21:37:04.094+00
2032	23	705	2021-02-09 21:37:04.292+00
2033	24	705	2021-02-09 21:37:04.292+00
2034	25	705	2021-02-09 21:37:04.292+00
2035	26	705	2021-02-09 21:37:04.292+00
2036	27	705	2021-02-09 21:37:04.292+00
2037	28	705	2021-02-09 21:37:04.292+00
2038	29	705	2021-02-09 21:37:04.292+00
2043	99	706	2021-02-09 21:37:04.519+00
2044	101	706	2021-02-09 21:37:04.519+00
2045	102	706	2021-02-09 21:37:04.519+00
2046	103	706	2021-02-09 21:37:04.519+00
2047	23	705	2021-02-09 21:59:07.088+00
2048	24	705	2021-02-09 21:59:07.088+00
2049	25	705	2021-02-09 21:59:07.088+00
2050	26	705	2021-02-09 21:59:07.088+00
2051	27	705	2021-02-09 21:59:07.088+00
2052	28	705	2021-02-09 21:59:07.088+00
2053	29	705	2021-02-09 21:59:07.088+00
2058	99	706	2021-02-09 21:59:07.726+00
2059	101	706	2021-02-09 21:59:07.726+00
2060	102	706	2021-02-09 21:59:07.726+00
2061	103	706	2021-02-09 21:59:07.726+00
2062	23	705	2021-02-09 21:59:14.866+00
2063	24	705	2021-02-09 21:59:14.866+00
2064	25	705	2021-02-09 21:59:14.866+00
2065	26	705	2021-02-09 21:59:14.866+00
2066	27	705	2021-02-09 21:59:14.866+00
2067	28	705	2021-02-09 21:59:14.866+00
2068	29	705	2021-02-09 21:59:14.866+00
2073	99	706	2021-02-09 21:59:15.143+00
2074	101	706	2021-02-09 21:59:15.143+00
2075	102	706	2021-02-09 21:59:15.143+00
2076	103	706	2021-02-09 21:59:15.143+00
2077	23	705	2021-02-09 21:59:15.455+00
2078	24	705	2021-02-09 21:59:15.455+00
2079	25	705	2021-02-09 21:59:15.455+00
2080	26	705	2021-02-09 21:59:15.455+00
2081	27	705	2021-02-09 21:59:15.455+00
2082	28	705	2021-02-09 21:59:15.455+00
2083	29	705	2021-02-09 21:59:15.455+00
2088	99	706	2021-02-09 21:59:15.747+00
2089	101	706	2021-02-09 21:59:15.747+00
2090	102	706	2021-02-09 21:59:15.747+00
2091	103	706	2021-02-09 21:59:15.747+00
2092	23	705	2021-02-09 21:59:38.619+00
2093	24	705	2021-02-09 21:59:38.619+00
2094	25	705	2021-02-09 21:59:38.619+00
2095	26	705	2021-02-09 21:59:38.619+00
2096	27	705	2021-02-09 21:59:38.619+00
2097	28	705	2021-02-09 21:59:38.619+00
2098	29	705	2021-02-09 21:59:38.619+00
2103	99	706	2021-02-09 21:59:39.042+00
2104	101	706	2021-02-09 21:59:39.042+00
2105	102	706	2021-02-09 21:59:39.042+00
2106	103	706	2021-02-09 21:59:39.042+00
2107	23	705	2021-02-09 21:59:46.044+00
2108	24	705	2021-02-09 21:59:46.044+00
2109	25	705	2021-02-09 21:59:46.044+00
2110	26	705	2021-02-09 21:59:46.044+00
2111	27	705	2021-02-09 21:59:46.044+00
2112	28	705	2021-02-09 21:59:46.044+00
2113	29	705	2021-02-09 21:59:46.044+00
2118	99	706	2021-02-09 21:59:46.363+00
2119	101	706	2021-02-09 21:59:46.363+00
2120	102	706	2021-02-09 21:59:46.363+00
2121	103	706	2021-02-09 21:59:46.363+00
2122	23	705	2021-02-09 21:59:46.66+00
2123	24	705	2021-02-09 21:59:46.66+00
2124	25	705	2021-02-09 21:59:46.66+00
2125	26	705	2021-02-09 21:59:46.66+00
2126	27	705	2021-02-09 21:59:46.66+00
2127	28	705	2021-02-09 21:59:46.66+00
2128	29	705	2021-02-09 21:59:46.66+00
2133	99	706	2021-02-09 21:59:46.93+00
2134	101	706	2021-02-09 21:59:46.93+00
2135	102	706	2021-02-09 21:59:46.93+00
2136	103	706	2021-02-09 21:59:46.93+00
2137	23	705	2021-02-09 22:00:15.961+00
2138	24	705	2021-02-09 22:00:15.961+00
2139	25	705	2021-02-09 22:00:15.961+00
2140	26	705	2021-02-09 22:00:15.961+00
2141	27	705	2021-02-09 22:00:15.961+00
2142	28	705	2021-02-09 22:00:15.961+00
2143	29	705	2021-02-09 22:00:15.961+00
2148	99	706	2021-02-09 22:00:16.529+00
2149	101	706	2021-02-09 22:00:16.529+00
2150	102	706	2021-02-09 22:00:16.529+00
2151	103	706	2021-02-09 22:00:16.529+00
2152	23	705	2021-02-09 22:00:23.643+00
2153	24	705	2021-02-09 22:00:23.643+00
2154	25	705	2021-02-09 22:00:23.643+00
2155	26	705	2021-02-09 22:00:23.643+00
2156	27	705	2021-02-09 22:00:23.643+00
2157	28	705	2021-02-09 22:00:23.643+00
2158	29	705	2021-02-09 22:00:23.643+00
2163	99	706	2021-02-09 22:00:23.934+00
2164	101	706	2021-02-09 22:00:23.934+00
2165	102	706	2021-02-09 22:00:23.934+00
2166	103	706	2021-02-09 22:00:23.934+00
2167	23	705	2021-02-09 22:00:24.249+00
2168	24	705	2021-02-09 22:00:24.249+00
2169	25	705	2021-02-09 22:00:24.249+00
2170	26	705	2021-02-09 22:00:24.249+00
2171	27	705	2021-02-09 22:00:24.249+00
2172	28	705	2021-02-09 22:00:24.249+00
2173	29	705	2021-02-09 22:00:24.249+00
2178	99	706	2021-02-09 22:00:24.543+00
2179	101	706	2021-02-09 22:00:24.543+00
2180	102	706	2021-02-09 22:00:24.543+00
2181	103	706	2021-02-09 22:00:24.543+00
2182	23	705	2021-02-09 22:00:46.431+00
2183	24	705	2021-02-09 22:00:46.431+00
2184	25	705	2021-02-09 22:00:46.431+00
2185	26	705	2021-02-09 22:00:46.431+00
2186	27	705	2021-02-09 22:00:46.431+00
2187	28	705	2021-02-09 22:00:46.431+00
2188	29	705	2021-02-09 22:00:46.431+00
2193	99	706	2021-02-09 22:00:46.981+00
2194	101	706	2021-02-09 22:00:46.981+00
2195	102	706	2021-02-09 22:00:46.981+00
2196	103	706	2021-02-09 22:00:46.981+00
2197	23	705	2021-02-09 22:00:53.876+00
2198	24	705	2021-02-09 22:00:53.876+00
2199	25	705	2021-02-09 22:00:53.876+00
2200	26	705	2021-02-09 22:00:53.876+00
2201	27	705	2021-02-09 22:00:53.876+00
2202	28	705	2021-02-09 22:00:53.876+00
2203	29	705	2021-02-09 22:00:53.876+00
2208	99	706	2021-02-09 22:00:54.136+00
2209	101	706	2021-02-09 22:00:54.136+00
2210	102	706	2021-02-09 22:00:54.136+00
2211	103	706	2021-02-09 22:00:54.136+00
2212	23	705	2021-02-09 22:00:54.419+00
2213	24	705	2021-02-09 22:00:54.419+00
2214	25	705	2021-02-09 22:00:54.419+00
2215	26	705	2021-02-09 22:00:54.419+00
2216	27	705	2021-02-09 22:00:54.419+00
2217	28	705	2021-02-09 22:00:54.419+00
2218	29	705	2021-02-09 22:00:54.419+00
2223	99	706	2021-02-09 22:00:54.726+00
2224	101	706	2021-02-09 22:00:54.726+00
2225	102	706	2021-02-09 22:00:54.726+00
2226	103	706	2021-02-09 22:00:54.726+00
2227	23	705	2021-02-10 00:17:00.351+00
2228	24	705	2021-02-10 00:17:00.351+00
2229	25	705	2021-02-10 00:17:00.351+00
2230	26	705	2021-02-10 00:17:00.351+00
2231	27	705	2021-02-10 00:17:00.351+00
2232	28	705	2021-02-10 00:17:00.351+00
2233	29	705	2021-02-10 00:17:00.351+00
2238	99	706	2021-02-10 00:17:02.461+00
2239	101	706	2021-02-10 00:17:02.461+00
2240	102	706	2021-02-10 00:17:02.461+00
2241	103	706	2021-02-10 00:17:02.461+00
2242	23	705	2021-02-10 00:17:42.542+00
2243	24	705	2021-02-10 00:17:42.542+00
2244	25	705	2021-02-10 00:17:42.542+00
2245	26	705	2021-02-10 00:17:42.542+00
2246	27	705	2021-02-10 00:17:42.542+00
2247	28	705	2021-02-10 00:17:42.542+00
2248	29	705	2021-02-10 00:17:42.542+00
2253	99	706	2021-02-10 00:17:44.251+00
2254	101	706	2021-02-10 00:17:44.251+00
2255	102	706	2021-02-10 00:17:44.251+00
2256	103	706	2021-02-10 00:17:44.251+00
2257	23	705	2021-02-10 00:17:45.865+00
2258	24	705	2021-02-10 00:17:45.865+00
2259	25	705	2021-02-10 00:17:45.865+00
2260	26	705	2021-02-10 00:17:45.865+00
2261	27	705	2021-02-10 00:17:45.865+00
2262	28	705	2021-02-10 00:17:45.865+00
2263	29	705	2021-02-10 00:17:45.865+00
2268	99	706	2021-02-10 00:17:47.615+00
2269	101	706	2021-02-10 00:17:47.615+00
2270	102	706	2021-02-10 00:17:47.615+00
2271	103	706	2021-02-10 00:17:47.615+00
2272	23	705	2021-02-10 00:19:25.029+00
2273	24	705	2021-02-10 00:19:25.029+00
2274	25	705	2021-02-10 00:19:25.029+00
2275	26	705	2021-02-10 00:19:25.029+00
2276	27	705	2021-02-10 00:19:25.029+00
2277	28	705	2021-02-10 00:19:25.029+00
2278	29	705	2021-02-10 00:19:25.029+00
2283	99	706	2021-02-10 00:19:27.112+00
2284	101	706	2021-02-10 00:19:27.112+00
2285	102	706	2021-02-10 00:19:27.112+00
2286	103	706	2021-02-10 00:19:27.112+00
2287	23	705	2021-02-10 00:20:07.382+00
2288	24	705	2021-02-10 00:20:07.382+00
2289	25	705	2021-02-10 00:20:07.382+00
2290	26	705	2021-02-10 00:20:07.382+00
2291	27	705	2021-02-10 00:20:07.382+00
2292	28	705	2021-02-10 00:20:07.382+00
2293	29	705	2021-02-10 00:20:07.382+00
2298	99	706	2021-02-10 00:20:09.164+00
2299	101	706	2021-02-10 00:20:09.164+00
2300	102	706	2021-02-10 00:20:09.164+00
2301	103	706	2021-02-10 00:20:09.164+00
2302	23	705	2021-02-10 00:20:10.707+00
2303	24	705	2021-02-10 00:20:10.707+00
2304	25	705	2021-02-10 00:20:10.707+00
2305	26	705	2021-02-10 00:20:10.707+00
2306	27	705	2021-02-10 00:20:10.707+00
2307	28	705	2021-02-10 00:20:10.707+00
2308	29	705	2021-02-10 00:20:10.707+00
2313	99	706	2021-02-10 00:20:12.427+00
2314	101	706	2021-02-10 00:20:12.427+00
2315	102	706	2021-02-10 00:20:12.427+00
2316	103	706	2021-02-10 00:20:12.427+00
2317	23	705	2021-02-10 00:21:52.062+00
2318	24	705	2021-02-10 00:21:52.062+00
2319	25	705	2021-02-10 00:21:52.062+00
2320	26	705	2021-02-10 00:21:52.062+00
2321	27	705	2021-02-10 00:21:52.062+00
2322	28	705	2021-02-10 00:21:52.062+00
2323	29	705	2021-02-10 00:21:52.062+00
2328	99	706	2021-02-10 00:21:54.254+00
2329	101	706	2021-02-10 00:21:54.254+00
2330	102	706	2021-02-10 00:21:54.254+00
2331	103	706	2021-02-10 00:21:54.254+00
2332	23	705	2021-02-10 00:22:36.139+00
2333	24	705	2021-02-10 00:22:36.139+00
2334	25	705	2021-02-10 00:22:36.139+00
2335	26	705	2021-02-10 00:22:36.139+00
2336	27	705	2021-02-10 00:22:36.139+00
2337	28	705	2021-02-10 00:22:36.139+00
2338	29	705	2021-02-10 00:22:36.139+00
2343	99	706	2021-02-10 00:22:37.934+00
2344	101	706	2021-02-10 00:22:37.934+00
2345	102	706	2021-02-10 00:22:37.934+00
2346	103	706	2021-02-10 00:22:37.934+00
2347	23	705	2021-02-10 00:22:39.579+00
2348	24	705	2021-02-10 00:22:39.579+00
2349	25	705	2021-02-10 00:22:39.579+00
2350	26	705	2021-02-10 00:22:39.579+00
2351	27	705	2021-02-10 00:22:39.579+00
2352	28	705	2021-02-10 00:22:39.579+00
2353	29	705	2021-02-10 00:22:39.579+00
2358	99	706	2021-02-10 00:22:41.371+00
2359	101	706	2021-02-10 00:22:41.371+00
2360	102	706	2021-02-10 00:22:41.371+00
2361	103	706	2021-02-10 00:22:41.371+00
2362	23	705	2021-02-10 00:24:13.739+00
2363	24	705	2021-02-10 00:24:13.739+00
2364	25	705	2021-02-10 00:24:13.739+00
2365	26	705	2021-02-10 00:24:13.739+00
2366	27	705	2021-02-10 00:24:13.739+00
2367	28	705	2021-02-10 00:24:13.739+00
2368	29	705	2021-02-10 00:24:13.739+00
2373	99	706	2021-02-10 00:24:15.686+00
2374	101	706	2021-02-10 00:24:15.686+00
2375	102	706	2021-02-10 00:24:15.686+00
2376	103	706	2021-02-10 00:24:15.686+00
2377	23	705	2021-02-10 00:24:56.869+00
2378	24	705	2021-02-10 00:24:56.869+00
2379	25	705	2021-02-10 00:24:56.869+00
2380	26	705	2021-02-10 00:24:56.869+00
2381	27	705	2021-02-10 00:24:56.869+00
2382	28	705	2021-02-10 00:24:56.869+00
2383	29	705	2021-02-10 00:24:56.869+00
2388	99	706	2021-02-10 00:24:58.636+00
2389	101	706	2021-02-10 00:24:58.636+00
2390	102	706	2021-02-10 00:24:58.636+00
2391	103	706	2021-02-10 00:24:58.636+00
2392	23	705	2021-02-10 00:25:00.255+00
2393	24	705	2021-02-10 00:25:00.255+00
2394	25	705	2021-02-10 00:25:00.255+00
2395	26	705	2021-02-10 00:25:00.255+00
2396	27	705	2021-02-10 00:25:00.255+00
2397	28	705	2021-02-10 00:25:00.255+00
2398	29	705	2021-02-10 00:25:00.255+00
2403	99	706	2021-02-10 00:25:02.04+00
2404	101	706	2021-02-10 00:25:02.04+00
2405	102	706	2021-02-10 00:25:02.04+00
2406	103	706	2021-02-10 00:25:02.04+00
\.


--
-- TOC entry 4479 (class 0 OID 22205)
-- Dependencies: 229
-- Data for Name: FieldValueElement; Type: TABLE DATA; Schema: dbo; Owner: -
--

COPY dbo."FieldValueElement" ("FieldValueElementId", "FieldValueId", "Order") FROM stdin;
10	8	1
11	9	1
12	10	1
13	11	1
14	12	1
15	13	1
16	14	1
17	14	2
18	14	3
19	15	1
20	16	1
21	17	1
22	18	1
23	19	1
24	20	1
25	21	1
26	21	2
27	21	3
28	22	1
29	23	1
30	24	1
31	25	1
32	26	1
33	27	1
34	28	1
35	28	2
36	28	3
41	33	1
42	34	1
43	35	1
44	36	1
45	36	2
46	36	3
47	37	1
48	38	1
49	39	1
50	40	1
51	41	1
52	42	1
53	43	1
54	43	2
55	43	3
60	48	1
61	49	1
62	50	1
63	51	1
64	51	2
65	51	3
66	52	1
67	53	1
68	54	1
69	55	1
70	56	1
71	57	1
72	58	1
73	58	2
74	58	3
79	63	1
80	64	1
81	65	1
82	66	1
83	66	2
84	66	3
89	78	1
90	79	1
91	80	1
92	81	1
93	81	2
94	81	3
95	82	1
96	83	1
97	84	1
98	85	1
99	86	1
100	87	1
101	88	1
102	88	2
103	88	3
108	93	1
109	94	1
110	95	1
111	96	1
112	96	2
113	96	3
114	97	1
115	98	1
116	99	1
117	100	1
118	101	1
119	102	1
120	103	1
121	103	2
122	103	3
127	108	1
128	109	1
129	110	1
130	111	1
131	111	2
132	111	3
133	112	1
134	113	1
135	114	1
136	115	1
137	116	1
138	117	1
139	118	1
140	118	2
141	118	3
146	123	1
147	124	1
148	125	1
149	126	1
150	126	2
151	126	3
152	127	1
153	128	1
154	129	1
155	130	1
156	131	1
157	132	1
158	133	1
159	133	2
160	133	3
165	138	1
166	139	1
167	140	1
168	141	1
169	141	2
170	141	3
171	142	1
172	143	1
173	144	1
174	145	1
175	146	1
176	147	1
177	148	1
178	148	2
179	148	3
184	153	1
185	154	1
186	155	1
187	156	1
188	156	2
189	156	3
190	157	1
191	158	1
192	159	1
193	160	1
194	161	1
195	162	1
196	163	1
197	163	2
198	163	3
589	472	1
590	473	1
591	474	1
203	168	1
204	169	1
205	170	1
206	171	1
207	171	2
208	171	3
209	172	1
210	173	1
211	174	1
212	175	1
213	176	1
214	177	1
215	178	1
216	178	2
217	178	3
222	183	1
223	184	1
224	185	1
225	186	1
226	186	2
227	186	3
228	187	1
229	188	1
230	189	1
231	190	1
232	191	1
233	192	1
234	193	1
235	193	2
236	193	3
241	198	1
242	199	1
243	200	1
244	201	1
245	201	2
246	201	3
247	202	1
248	203	1
249	204	1
250	205	1
251	206	1
252	207	1
253	208	1
254	208	2
255	208	3
260	213	1
261	214	1
262	215	1
263	216	1
264	216	2
265	216	3
266	217	1
267	218	1
268	219	1
269	220	1
270	221	1
271	222	1
272	223	1
273	223	2
274	223	3
279	228	1
280	229	1
281	230	1
282	231	1
283	231	2
284	231	3
285	232	1
286	233	1
287	234	1
288	235	1
289	236	1
290	237	1
291	238	1
292	238	2
293	238	3
298	243	1
299	244	1
300	245	1
301	246	1
302	246	2
303	246	3
304	247	1
305	248	1
306	249	1
307	250	1
308	251	1
309	252	1
310	253	1
311	253	2
312	253	3
317	258	1
318	259	1
319	260	1
320	261	1
321	261	2
322	261	3
323	262	1
324	263	1
325	264	1
326	265	1
327	266	1
328	267	1
329	268	1
330	268	2
331	268	3
336	273	1
337	274	1
338	275	1
339	276	1
340	276	2
341	276	3
342	277	1
343	278	1
344	279	1
345	280	1
346	281	1
347	282	1
348	283	1
349	283	2
350	283	3
355	288	1
356	289	1
357	290	1
358	291	1
359	291	2
360	291	3
361	292	1
362	293	1
363	294	1
364	295	1
365	296	1
366	297	1
367	298	1
368	298	2
369	298	3
374	303	1
375	304	1
376	305	1
377	306	1
378	306	2
379	306	3
380	307	1
381	308	1
382	309	1
383	310	1
384	311	1
385	312	1
386	313	1
387	313	2
388	313	3
393	318	1
394	319	1
395	320	1
396	321	1
397	321	2
398	321	3
399	322	1
400	323	1
401	324	1
402	325	1
403	326	1
404	327	1
405	328	1
406	328	2
407	328	3
592	475	1
593	476	1
594	477	1
595	478	1
412	333	1
413	334	1
414	335	1
415	336	1
416	336	2
417	336	3
418	337	1
419	338	1
420	339	1
421	340	1
422	341	1
423	342	1
424	343	1
425	343	2
426	343	3
431	348	1
432	349	1
433	350	1
434	351	1
435	351	2
436	351	3
437	352	1
438	353	1
439	354	1
440	355	1
441	356	1
442	357	1
443	358	1
444	358	2
445	358	3
450	363	1
451	364	1
452	365	1
453	366	1
454	366	2
455	366	3
456	367	1
457	368	1
458	369	1
459	370	1
460	371	1
461	372	1
462	373	1
463	373	2
464	373	3
469	378	1
470	379	1
471	380	1
472	381	1
473	381	2
474	381	3
475	382	1
476	383	1
477	384	1
478	385	1
479	386	1
480	387	1
481	388	1
482	388	2
483	388	3
488	393	1
489	394	1
490	395	1
491	396	1
492	396	2
493	396	3
494	397	1
495	398	1
496	399	1
497	400	1
498	401	1
499	402	1
500	403	1
501	403	2
502	403	3
507	408	1
508	409	1
509	410	1
510	411	1
511	411	2
512	411	3
513	412	1
514	413	1
515	414	1
516	415	1
517	416	1
518	417	1
519	418	1
520	418	2
521	418	3
526	423	1
527	424	1
528	425	1
529	426	1
530	426	2
531	426	3
532	427	1
533	428	1
534	429	1
535	430	1
536	431	1
537	432	1
538	433	1
539	433	2
540	433	3
545	438	1
546	439	1
547	440	1
548	441	1
549	441	2
550	441	3
551	442	1
552	443	1
553	444	1
554	445	1
555	446	1
556	447	1
557	448	1
558	448	2
559	448	3
564	453	1
565	454	1
566	455	1
567	456	1
568	456	2
569	456	3
570	457	1
571	458	1
572	459	1
573	460	1
574	461	1
575	462	1
576	463	1
577	463	2
578	463	3
583	468	1
584	469	1
585	470	1
586	471	1
587	471	2
588	471	3
596	478	2
597	478	3
602	483	1
603	484	1
604	485	1
605	486	1
606	486	2
607	486	3
608	487	1
609	488	1
610	489	1
611	490	1
612	491	1
613	492	1
614	493	1
615	493	2
616	493	3
621	498	1
622	499	1
623	500	1
624	501	1
625	501	2
626	501	3
627	502	1
628	503	1
629	504	1
630	505	1
631	506	1
632	507	1
633	508	1
634	508	2
635	508	3
640	513	1
641	514	1
642	515	1
643	516	1
644	516	2
645	516	3
646	517	1
647	518	1
648	519	1
649	520	1
650	521	1
651	522	1
652	523	1
653	523	2
654	523	3
659	528	1
660	529	1
661	530	1
662	531	1
663	531	2
664	531	3
665	532	1
666	533	1
667	534	1
668	535	1
669	536	1
670	537	1
671	538	1
672	538	2
673	538	3
678	543	1
679	544	1
680	545	1
681	546	1
682	546	2
683	546	3
684	547	1
685	548	1
686	549	1
687	550	1
688	551	1
689	552	1
690	553	1
691	553	2
692	553	3
697	558	1
698	559	1
699	560	1
700	561	1
701	561	2
702	561	3
703	562	1
704	563	1
705	564	1
706	565	1
707	566	1
708	567	1
709	568	1
710	568	2
711	568	3
716	573	1
717	574	1
718	575	1
719	576	1
720	576	2
721	576	3
722	577	1
723	578	1
724	579	1
725	580	1
726	581	1
727	582	1
728	583	1
729	583	2
730	583	3
735	588	1
736	589	1
737	590	1
738	591	1
739	591	2
740	591	3
741	592	1
742	593	1
743	594	1
744	595	1
745	596	1
746	597	1
747	598	1
748	598	2
749	598	3
754	603	1
755	604	1
756	605	1
757	606	1
758	606	2
759	606	3
760	607	1
761	608	1
762	609	1
763	610	1
764	611	1
765	612	1
766	613	1
767	613	2
768	613	3
773	618	1
774	619	1
775	620	1
776	621	1
777	621	2
778	621	3
779	622	1
780	623	1
781	624	1
782	625	1
783	626	1
784	627	1
785	628	1
786	628	2
787	628	3
792	633	1
793	634	1
794	635	1
795	636	1
796	636	2
797	636	3
798	637	1
799	638	1
800	639	1
801	640	1
802	641	1
803	642	1
804	643	1
805	643	2
806	643	3
811	648	1
812	649	1
813	650	1
814	651	1
815	651	2
816	651	3
817	652	1
818	653	1
819	654	1
820	655	1
821	656	1
822	657	1
823	658	1
824	658	2
825	658	3
830	663	1
831	664	1
832	665	1
833	666	1
834	666	2
835	666	3
836	667	1
837	668	1
838	669	1
839	670	1
840	671	1
841	672	1
842	673	1
843	673	2
844	673	3
849	678	1
850	679	1
851	680	1
852	681	1
853	681	2
854	681	3
855	682	1
856	683	1
857	684	1
858	685	1
859	686	1
860	687	1
861	688	1
862	688	2
863	688	3
868	693	1
869	694	1
870	695	1
871	696	1
872	696	2
873	696	3
874	697	1
875	698	1
876	699	1
877	700	1
878	701	1
879	702	1
880	703	1
881	703	2
882	703	3
887	708	1
888	709	1
889	710	1
890	711	1
891	711	2
892	711	3
893	712	1
894	713	1
895	714	1
896	715	1
897	716	1
898	717	1
899	718	1
900	718	2
901	718	3
906	723	1
907	724	1
908	725	1
909	726	1
910	726	2
911	726	3
912	727	1
913	728	1
914	729	1
915	730	1
916	731	1
917	732	1
918	733	1
919	733	2
920	733	3
925	738	1
926	739	1
927	740	1
928	741	1
929	741	2
930	741	3
931	742	1
932	743	1
933	744	1
934	745	1
935	746	1
936	747	1
937	748	1
938	748	2
939	748	3
944	753	1
945	754	1
946	755	1
947	756	1
948	756	2
949	756	3
950	757	1
951	758	1
952	759	1
953	760	1
954	761	1
955	762	1
956	763	1
957	763	2
958	763	3
963	768	1
964	769	1
965	770	1
966	771	1
967	771	2
968	771	3
969	772	1
970	773	1
971	774	1
972	775	1
973	776	1
974	777	1
975	778	1
976	778	2
977	778	3
2660	2107	1
2661	2108	1
2662	2109	1
982	783	1
983	784	1
984	785	1
985	786	1
986	786	2
987	786	3
988	787	1
989	788	1
990	789	1
991	790	1
992	791	1
993	792	1
994	793	1
995	793	2
996	793	3
1001	798	1
1002	799	1
1003	800	1
1004	801	1
1005	801	2
1006	801	3
1007	802	1
1008	803	1
1009	804	1
1010	805	1
1011	806	1
1012	807	1
1013	808	1
1014	808	2
1015	808	3
1020	813	1
1021	814	1
1022	815	1
1023	816	1
1024	816	2
1025	816	3
1026	817	1
1027	818	1
1028	819	1
1029	820	1
1030	821	1
1031	822	1
1032	823	1
1033	823	2
1034	823	3
1039	828	1
1040	829	1
1041	830	1
1042	831	1
1043	831	2
1044	831	3
1045	832	1
1046	833	1
1047	834	1
1048	835	1
1049	836	1
1050	837	1
1051	838	1
1052	838	2
1053	838	3
1058	843	1
1059	844	1
1060	845	1
1061	846	1
1062	846	2
1063	846	3
1064	847	1
1065	848	1
1066	849	1
1067	850	1
1068	851	1
1069	852	1
1070	853	1
1071	853	2
1072	853	3
1077	858	1
1078	859	1
1079	860	1
1080	861	1
1081	861	2
1082	861	3
1083	862	1
1084	863	1
1085	864	1
1086	865	1
1087	866	1
1088	867	1
1089	868	1
1090	868	2
1091	868	3
1096	873	1
1097	874	1
1098	875	1
1099	876	1
1100	876	2
1101	876	3
1102	877	1
1103	878	1
1104	879	1
1105	880	1
1106	881	1
1107	882	1
1108	883	1
1109	883	2
1110	883	3
1115	888	1
1116	889	1
1117	890	1
1118	891	1
1119	891	2
1120	891	3
1121	892	1
1122	893	1
1123	894	1
1124	895	1
1125	896	1
1126	897	1
1127	898	1
1128	898	2
1129	898	3
1134	903	1
1135	904	1
1136	905	1
1137	906	1
1138	906	2
1139	906	3
1140	907	1
1141	908	1
1142	909	1
1143	910	1
1144	911	1
1145	912	1
1146	913	1
1147	913	2
1148	913	3
1153	918	1
1154	919	1
1155	920	1
1156	921	1
1157	921	2
1158	921	3
1159	922	1
1160	923	1
1161	924	1
1162	925	1
1163	926	1
1164	927	1
1165	928	1
1166	928	2
1167	928	3
1172	933	1
1173	934	1
1174	935	1
1175	936	1
1176	936	2
1177	936	3
1178	937	1
1179	938	1
1180	939	1
1181	940	1
1182	941	1
1183	942	1
1184	943	1
1185	943	2
1186	943	3
2663	2110	1
2664	2111	1
2665	2112	1
2666	2113	1
1191	948	1
1192	949	1
1193	950	1
1194	951	1
1195	951	2
1196	951	3
1197	952	1
1198	953	1
1199	954	1
1200	955	1
1201	956	1
1202	957	1
1203	958	1
1204	958	2
1205	958	3
1210	963	1
1211	964	1
1212	965	1
1213	966	1
1214	966	2
1215	966	3
1216	967	1
1217	968	1
1218	969	1
1219	970	1
1220	971	1
1221	972	1
1222	973	1
1223	973	2
1224	973	3
1229	978	1
1230	979	1
1231	980	1
1232	981	1
1233	981	2
1234	981	3
1235	982	1
1236	983	1
1237	984	1
1238	985	1
1239	986	1
1240	987	1
1241	988	1
1242	988	2
1243	988	3
1248	993	1
1249	994	1
1250	995	1
1251	996	1
1252	996	2
1253	996	3
1254	997	1
1255	998	1
1256	999	1
1257	1000	1
1258	1001	1
1259	1002	1
1260	1003	1
1261	1003	2
1262	1003	3
1267	1008	1
1268	1009	1
1269	1010	1
1270	1011	1
1271	1011	2
1272	1011	3
1273	1012	1
1274	1013	1
1275	1014	1
1276	1015	1
1277	1016	1
1278	1017	1
1279	1018	1
1280	1018	2
1281	1018	3
1286	1023	1
1287	1024	1
1288	1025	1
1289	1026	1
1290	1026	2
1291	1026	3
1292	1027	1
1293	1028	1
1294	1029	1
1295	1030	1
1296	1031	1
1297	1032	1
1298	1033	1
1299	1033	2
1300	1033	3
1305	1038	1
1306	1039	1
1307	1040	1
1308	1041	1
1309	1041	2
1310	1041	3
1311	1042	1
1312	1043	1
1313	1044	1
1314	1045	1
1315	1046	1
1316	1047	1
1317	1048	1
1318	1048	2
1319	1048	3
1324	1053	1
1325	1054	1
1326	1055	1
1327	1056	1
1328	1056	2
1329	1056	3
1330	1057	1
1331	1058	1
1332	1059	1
1333	1060	1
1334	1061	1
1335	1062	1
1336	1063	1
1337	1063	2
1338	1063	3
1343	1068	1
1344	1069	1
1345	1070	1
1346	1071	1
1347	1071	2
1348	1071	3
1349	1072	1
1350	1073	1
1351	1074	1
1352	1075	1
1353	1076	1
1354	1077	1
1355	1078	1
1356	1078	2
1357	1078	3
1362	1083	1
1363	1084	1
1364	1085	1
1365	1086	1
1366	1086	2
1367	1086	3
1368	1087	1
1369	1088	1
1370	1089	1
1371	1090	1
1372	1091	1
1373	1092	1
1374	1093	1
1375	1093	2
1376	1093	3
2667	2113	2
2668	2113	3
1381	1098	1
1382	1099	1
1383	1100	1
1384	1101	1
1385	1101	2
1386	1101	3
1387	1102	1
1388	1103	1
1389	1104	1
1390	1105	1
1391	1106	1
1392	1107	1
1393	1108	1
1394	1108	2
1395	1108	3
1400	1113	1
1401	1114	1
1402	1115	1
1403	1116	1
1404	1116	2
1405	1116	3
1406	1117	1
1407	1118	1
1408	1119	1
1409	1120	1
1410	1121	1
1411	1122	1
1412	1123	1
1413	1123	2
1414	1123	3
1419	1128	1
1420	1129	1
1421	1130	1
1422	1131	1
1423	1131	2
1424	1131	3
1425	1132	1
1426	1133	1
1427	1134	1
1428	1135	1
1429	1136	1
1430	1137	1
1431	1138	1
1432	1138	2
1433	1138	3
1438	1143	1
1439	1144	1
1440	1145	1
1441	1146	1
1442	1146	2
1443	1146	3
1444	1147	1
1445	1148	1
1446	1149	1
1447	1150	1
1448	1151	1
1449	1152	1
1450	1153	1
1451	1153	2
1452	1153	3
1457	1158	1
1458	1159	1
1459	1160	1
1460	1161	1
1461	1161	2
1462	1161	3
1463	1162	1
1464	1163	1
1465	1164	1
1466	1165	1
1467	1166	1
1468	1167	1
1469	1168	1
1470	1168	2
1471	1168	3
1476	1173	1
1477	1174	1
1478	1175	1
1479	1176	1
1480	1176	2
1481	1176	3
1482	1177	1
1483	1178	1
1484	1179	1
1485	1180	1
1486	1181	1
1487	1182	1
1488	1183	1
1489	1183	2
1490	1183	3
1495	1188	1
1496	1189	1
1497	1190	1
1498	1191	1
1499	1191	2
1500	1191	3
1501	1192	1
1502	1193	1
1503	1194	1
1504	1195	1
1505	1196	1
1506	1197	1
1507	1198	1
1508	1198	2
1509	1198	3
1514	1203	1
1515	1204	1
1516	1205	1
1517	1206	1
1518	1206	2
1519	1206	3
1520	1207	1
1521	1208	1
1522	1209	1
1523	1210	1
1524	1211	1
1525	1212	1
1526	1213	1
1527	1213	2
1528	1213	3
1533	1218	1
1534	1219	1
1535	1220	1
1536	1221	1
1537	1221	2
1538	1221	3
1539	1222	1
1540	1223	1
1541	1224	1
1542	1225	1
1543	1226	1
1544	1227	1
1545	1228	1
1546	1228	2
1547	1228	3
1552	1233	1
1553	1234	1
1554	1235	1
1555	1236	1
1556	1236	2
1557	1236	3
1558	1237	1
1559	1238	1
1560	1239	1
1561	1240	1
1562	1241	1
1563	1242	1
1564	1243	1
1565	1243	2
1566	1243	3
2673	2118	1
2674	2119	1
1571	1248	1
1572	1249	1
1573	1250	1
1574	1251	1
1575	1251	2
1576	1251	3
1577	1252	1
1578	1253	1
1579	1254	1
1580	1255	1
1581	1256	1
1582	1257	1
1583	1258	1
1584	1258	2
1585	1258	3
1590	1263	1
1591	1264	1
1592	1265	1
1593	1266	1
1594	1266	2
1595	1266	3
1596	1267	1
1597	1268	1
1598	1269	1
1599	1270	1
1600	1271	1
1601	1272	1
1602	1273	1
1603	1273	2
1604	1273	3
1609	1278	1
1610	1279	1
1611	1280	1
1612	1281	1
1613	1281	2
1614	1281	3
1615	1282	1
1616	1283	1
1617	1284	1
1618	1285	1
1619	1286	1
1620	1287	1
1621	1288	1
1622	1288	2
1623	1288	3
1628	1293	1
1629	1294	1
1630	1295	1
1631	1296	1
1632	1296	2
1633	1296	3
1634	1297	1
1635	1298	1
1636	1299	1
1637	1300	1
1638	1301	1
1639	1302	1
1640	1303	1
1641	1303	2
1642	1303	3
1647	1308	1
1648	1309	1
1649	1310	1
1650	1311	1
1651	1311	2
1652	1311	3
1653	1312	1
1654	1313	1
1655	1314	1
1656	1315	1
1657	1316	1
1658	1317	1
1659	1318	1
1660	1318	2
1661	1318	3
1666	1323	1
1667	1324	1
1668	1325	1
1669	1326	1
1670	1326	2
1671	1326	3
1672	1327	1
1673	1328	1
1674	1329	1
1675	1330	1
1676	1331	1
1677	1332	1
1678	1333	1
1679	1333	2
1680	1333	3
1685	1338	1
1686	1339	1
1687	1340	1
1688	1341	1
1689	1341	2
1690	1341	3
1691	1342	1
1692	1343	1
1693	1344	1
1694	1345	1
1695	1346	1
1696	1347	1
1697	1348	1
1698	1348	2
1699	1348	3
1704	1353	1
1705	1354	1
1706	1355	1
1707	1356	1
1708	1356	2
1709	1356	3
1710	1357	1
1711	1358	1
1712	1359	1
1713	1360	1
1714	1361	1
1715	1362	1
1716	1363	1
1717	1363	2
1718	1363	3
1723	1368	1
1724	1369	1
1725	1370	1
1726	1371	1
1727	1371	2
1728	1371	3
1729	1372	1
1730	1373	1
1731	1374	1
1732	1375	1
1733	1376	1
1734	1377	1
1735	1378	1
1736	1378	2
1737	1378	3
1742	1383	1
1743	1384	1
1744	1385	1
1745	1386	1
1746	1386	2
1747	1386	3
1748	1387	1
1749	1388	1
1750	1389	1
1751	1390	1
1752	1391	1
1753	1392	1
1754	1393	1
1755	1393	2
1756	1393	3
2675	2120	1
2676	2121	1
2677	2121	2
2678	2121	3
1761	1398	1
1762	1399	1
1763	1400	1
1764	1401	1
1765	1401	2
1766	1401	3
1767	1402	1
1768	1403	1
1769	1404	1
1770	1405	1
1771	1406	1
1772	1407	1
1773	1408	1
1774	1408	2
1775	1408	3
1780	1413	1
1781	1414	1
1782	1415	1
1783	1416	1
1784	1416	2
1785	1416	3
1786	1417	1
1787	1418	1
1788	1419	1
1789	1420	1
1790	1421	1
1791	1422	1
1792	1423	1
1793	1423	2
1794	1423	3
1799	1428	1
1800	1429	1
1801	1430	1
1802	1431	1
1803	1431	2
1804	1431	3
1805	1432	1
1806	1433	1
1807	1434	1
1808	1435	1
1809	1436	1
1810	1437	1
1811	1438	1
1812	1438	2
1813	1438	3
1818	1443	1
1819	1444	1
1820	1445	1
1821	1446	1
1822	1446	2
1823	1446	3
1824	1447	1
1825	1448	1
1826	1449	1
1827	1450	1
1828	1451	1
1829	1452	1
1830	1453	1
1831	1453	2
1832	1453	3
1837	1458	1
1838	1459	1
1839	1460	1
1840	1461	1
1841	1461	2
1842	1461	3
1843	1462	1
1844	1463	1
1845	1464	1
1846	1465	1
1847	1466	1
1848	1467	1
1849	1468	1
1850	1468	2
1851	1468	3
1856	1473	1
1857	1474	1
1858	1475	1
1859	1476	1
1860	1476	2
1861	1476	3
1862	1477	1
1863	1478	1
1864	1479	1
1865	1480	1
1866	1481	1
1867	1482	1
1868	1483	1
1869	1483	2
1870	1483	3
1875	1488	1
1876	1489	1
1877	1490	1
1878	1491	1
1879	1491	2
1880	1491	3
1881	1492	1
1882	1493	1
1883	1494	1
1884	1495	1
1885	1496	1
1886	1497	1
1887	1498	1
1888	1498	2
1889	1498	3
1894	1503	1
1895	1504	1
1896	1505	1
1897	1506	1
1898	1506	2
1899	1506	3
1900	1507	1
1901	1508	1
1902	1509	1
1903	1510	1
1904	1511	1
1905	1512	1
1906	1513	1
1907	1513	2
1908	1513	3
1913	1518	1
1914	1519	1
1915	1520	1
1916	1521	1
1917	1521	2
1918	1521	3
1919	1522	1
1920	1523	1
1921	1524	1
1922	1525	1
1923	1526	1
1924	1527	1
1925	1528	1
1926	1528	2
1927	1528	3
1932	1533	1
1933	1534	1
1934	1535	1
1935	1536	1
1936	1536	2
1937	1536	3
1938	1537	1
1939	1538	1
1940	1539	1
1941	1540	1
1942	1541	1
1943	1542	1
1944	1543	1
1945	1543	2
1946	1543	3
2679	2122	1
2680	2123	1
2681	2124	1
2682	2125	1
1951	1548	1
1952	1549	1
1953	1550	1
1954	1551	1
1955	1551	2
1956	1551	3
1957	1552	1
1958	1553	1
1959	1554	1
1960	1555	1
1961	1556	1
1962	1557	1
1963	1558	1
1964	1558	2
1965	1558	3
1970	1563	1
1971	1564	1
1972	1565	1
1973	1566	1
1974	1566	2
1975	1566	3
1976	1567	1
1977	1568	1
1978	1569	1
1979	1570	1
1980	1571	1
1981	1572	1
1982	1573	1
1983	1573	2
1984	1573	3
1989	1578	1
1990	1579	1
1991	1580	1
1992	1581	1
1993	1581	2
1994	1581	3
1995	1582	1
1996	1583	1
1997	1584	1
1998	1585	1
1999	1586	1
2000	1587	1
2001	1588	1
2002	1588	2
2003	1588	3
2008	1593	1
2009	1594	1
2010	1595	1
2011	1596	1
2012	1596	2
2013	1596	3
2014	1597	1
2015	1598	1
2016	1599	1
2017	1600	1
2018	1601	1
2019	1602	1
2020	1603	1
2021	1603	2
2022	1603	3
2027	1608	1
2028	1609	1
2029	1610	1
2030	1611	1
2031	1611	2
2032	1611	3
2033	1612	1
2034	1613	1
2035	1614	1
2036	1615	1
2037	1616	1
2038	1617	1
2039	1618	1
2040	1618	2
2041	1618	3
2046	1623	1
2047	1624	1
2048	1625	1
2049	1626	1
2050	1626	2
2051	1626	3
2052	1627	1
2053	1628	1
2054	1629	1
2055	1630	1
2056	1631	1
2057	1632	1
2058	1633	1
2059	1633	2
2060	1633	3
2065	1638	1
2066	1639	1
2067	1640	1
2068	1641	1
2069	1641	2
2070	1641	3
2071	1642	1
2072	1643	1
2073	1644	1
2074	1645	1
2075	1646	1
2076	1647	1
2077	1648	1
2078	1648	2
2079	1648	3
2084	1653	1
2085	1654	1
2086	1655	1
2087	1656	1
2088	1656	2
2089	1656	3
2090	1657	1
2091	1658	1
2092	1659	1
2093	1660	1
2094	1661	1
2095	1662	1
2096	1663	1
2097	1663	2
2098	1663	3
2103	1668	1
2104	1669	1
2105	1670	1
2106	1671	1
2107	1671	2
2108	1671	3
2109	1672	1
2110	1673	1
2111	1674	1
2112	1675	1
2113	1676	1
2114	1677	1
2115	1678	1
2116	1678	2
2117	1678	3
2122	1683	1
2123	1684	1
2124	1685	1
2125	1686	1
2126	1686	2
2127	1686	3
2128	1687	1
2129	1688	1
2130	1689	1
2131	1690	1
2132	1691	1
2133	1692	1
2134	1693	1
2135	1693	2
2136	1693	3
2683	2126	1
2684	2127	1
2685	2128	1
2686	2128	2
2141	1698	1
2142	1699	1
2143	1700	1
2144	1701	1
2145	1701	2
2146	1701	3
2147	1702	1
2148	1703	1
2149	1704	1
2150	1705	1
2151	1706	1
2152	1707	1
2153	1708	1
2154	1708	2
2155	1708	3
2160	1713	1
2161	1714	1
2162	1715	1
2163	1716	1
2164	1716	2
2165	1716	3
2166	1717	1
2167	1718	1
2168	1719	1
2169	1720	1
2170	1721	1
2171	1722	1
2172	1723	1
2173	1723	2
2174	1723	3
2179	1728	1
2180	1729	1
2181	1730	1
2182	1731	1
2183	1731	2
2184	1731	3
2185	1732	1
2186	1733	1
2187	1734	1
2188	1735	1
2189	1736	1
2190	1737	1
2191	1738	1
2192	1738	2
2193	1738	3
2198	1743	1
2199	1744	1
2200	1745	1
2201	1746	1
2202	1746	2
2203	1746	3
2204	1747	1
2205	1748	1
2206	1749	1
2207	1750	1
2208	1751	1
2209	1752	1
2210	1753	1
2211	1753	2
2212	1753	3
2217	1758	1
2218	1759	1
2219	1760	1
2220	1761	1
2221	1761	2
2222	1761	3
2223	1762	1
2224	1763	1
2225	1764	1
2226	1765	1
2227	1766	1
2228	1767	1
2229	1768	1
2230	1768	2
2231	1768	3
2236	1773	1
2237	1774	1
2238	1775	1
2239	1776	1
2240	1776	2
2241	1776	3
2242	1777	1
2243	1778	1
2244	1779	1
2245	1780	1
2246	1781	1
2247	1782	1
2248	1783	1
2249	1783	2
2250	1783	3
2255	1788	1
2256	1789	1
2257	1790	1
2258	1791	1
2259	1791	2
2260	1791	3
2261	1792	1
2262	1793	1
2263	1794	1
2264	1795	1
2265	1796	1
2266	1797	1
2267	1798	1
2268	1798	2
2269	1798	3
2274	1803	1
2275	1804	1
2276	1805	1
2277	1806	1
2278	1806	2
2279	1806	3
2280	1807	1
2281	1808	1
2282	1809	1
2283	1810	1
2284	1811	1
2285	1812	1
2286	1813	1
2287	1813	2
2288	1813	3
2293	1818	1
2294	1819	1
2295	1820	1
2296	1821	1
2297	1821	2
2298	1821	3
2299	1822	1
2300	1823	1
2301	1824	1
2302	1825	1
2303	1826	1
2304	1827	1
2305	1828	1
2306	1828	2
2307	1828	3
2312	1833	1
2313	1834	1
2314	1835	1
2315	1836	1
2316	1836	2
2317	1836	3
2318	1837	1
2319	1838	1
2320	1839	1
2321	1840	1
2322	1841	1
2323	1842	1
2324	1843	1
2325	1843	2
2326	1843	3
2687	2128	3
2331	1848	1
2332	1849	1
2333	1850	1
2334	1851	1
2335	1851	2
2336	1851	3
2337	1852	1
2338	1853	1
2339	1854	1
2340	1855	1
2341	1856	1
2342	1857	1
2343	1858	1
2344	1858	2
2345	1858	3
2350	1863	1
2351	1864	1
2352	1865	1
2353	1866	1
2354	1866	2
2355	1866	3
2356	1867	1
2357	1868	1
2358	1869	1
2359	1870	1
2360	1871	1
2361	1872	1
2362	1873	1
2363	1873	2
2364	1873	3
2369	1878	1
2370	1879	1
2371	1880	1
2372	1881	1
2373	1881	2
2374	1881	3
2375	1882	1
2376	1883	1
2377	1884	1
2378	1885	1
2379	1886	1
2380	1887	1
2381	1888	1
2382	1888	2
2383	1888	3
2388	1893	1
2389	1894	1
2390	1895	1
2391	1896	1
2392	1896	2
2393	1896	3
2394	1897	1
2395	1898	1
2396	1899	1
2397	1900	1
2398	1901	1
2399	1902	1
2400	1903	1
2401	1903	2
2402	1903	3
2407	1908	1
2408	1909	1
2409	1910	1
2410	1911	1
2411	1911	2
2412	1911	3
2413	1912	1
2414	1913	1
2415	1914	1
2416	1915	1
2417	1916	1
2418	1917	1
2419	1918	1
2420	1918	2
2421	1918	3
2426	1923	1
2427	1924	1
2428	1925	1
2429	1926	1
2430	1926	2
2431	1926	3
2432	1927	1
2433	1928	1
2434	1929	1
2435	1930	1
2436	1931	1
2437	1932	1
2438	1933	1
2439	1933	2
2440	1933	3
2445	1938	1
2446	1939	1
2447	1940	1
2448	1941	1
2449	1941	2
2450	1941	3
2451	1942	1
2452	1943	1
2453	1944	1
2454	1945	1
2455	1946	1
2456	1947	1
2457	1948	1
2458	1948	2
2459	1948	3
2464	1953	1
2465	1954	1
2466	1955	1
2467	1956	1
2468	1956	2
2469	1956	3
2470	1957	1
2471	1958	1
2472	1959	1
2473	1960	1
2474	1961	1
2475	1962	1
2476	1963	1
2477	1963	2
2478	1963	3
2483	1968	1
2484	1969	1
2485	1970	1
2486	1971	1
2487	1971	2
2488	1971	3
2489	1972	1
2490	1973	1
2491	1974	1
2492	1975	1
2493	1976	1
2494	1977	1
2495	1978	1
2496	1978	2
2497	1978	3
2502	1983	1
2503	1984	1
2504	1985	1
2505	1986	1
2506	1986	2
2507	1986	3
2508	1987	1
2509	1988	1
2510	1989	1
2511	1990	1
2512	1991	1
2513	1992	1
2514	1993	1
2515	1993	2
2516	1993	3
2692	2133	1
2693	2134	1
2694	2135	1
2521	1998	1
2522	1999	1
2523	2000	1
2524	2001	1
2525	2001	2
2526	2001	3
2527	2002	1
2528	2003	1
2529	2004	1
2530	2005	1
2531	2006	1
2532	2007	1
2533	2008	1
2534	2008	2
2535	2008	3
2695	2136	1
2696	2136	2
2697	2136	3
2698	2137	1
2540	2013	1
2541	2014	1
2542	2015	1
2543	2016	1
2544	2016	2
2545	2016	3
2546	2017	1
2547	2018	1
2548	2019	1
2549	2020	1
2550	2021	1
2551	2022	1
2552	2023	1
2553	2023	2
2554	2023	3
2699	2138	1
2700	2139	1
2701	2140	1
2702	2141	1
2559	2028	1
2560	2029	1
2561	2030	1
2562	2031	1
2563	2031	2
2564	2031	3
2565	2032	1
2566	2033	1
2567	2034	1
2568	2035	1
2569	2036	1
2570	2037	1
2571	2038	1
2572	2038	2
2573	2038	3
2703	2142	1
2704	2143	1
2705	2143	2
2706	2143	3
2578	2043	1
2579	2044	1
2580	2045	1
2581	2046	1
2582	2046	2
2583	2046	3
2584	2047	1
2585	2048	1
2586	2049	1
2587	2050	1
2588	2051	1
2589	2052	1
2590	2053	1
2591	2053	2
2592	2053	3
2597	2058	1
2598	2059	1
2599	2060	1
2600	2061	1
2601	2061	2
2602	2061	3
2603	2062	1
2604	2063	1
2605	2064	1
2606	2065	1
2607	2066	1
2608	2067	1
2609	2068	1
2610	2068	2
2611	2068	3
2711	2148	1
2712	2149	1
2713	2150	1
2714	2151	1
2616	2073	1
2617	2074	1
2618	2075	1
2619	2076	1
2620	2076	2
2621	2076	3
2622	2077	1
2623	2078	1
2624	2079	1
2625	2080	1
2626	2081	1
2627	2082	1
2628	2083	1
2629	2083	2
2630	2083	3
2715	2151	2
2716	2151	3
2717	2152	1
2718	2153	1
2635	2088	1
2636	2089	1
2637	2090	1
2638	2091	1
2639	2091	2
2640	2091	3
2641	2092	1
2642	2093	1
2643	2094	1
2644	2095	1
2645	2096	1
2646	2097	1
2647	2098	1
2648	2098	2
2649	2098	3
2719	2154	1
2720	2155	1
2721	2156	1
2722	2157	1
2654	2103	1
2655	2104	1
2656	2105	1
2657	2106	1
2658	2106	2
2659	2106	3
2723	2158	1
2724	2158	2
2725	2158	3
2730	2163	1
2731	2164	1
2732	2165	1
2733	2166	1
2734	2166	2
2735	2166	3
2736	2167	1
2737	2168	1
2738	2169	1
2739	2170	1
2740	2171	1
2741	2172	1
2742	2173	1
2743	2173	2
2744	2173	3
2749	2178	1
2750	2179	1
2751	2180	1
2752	2181	1
2753	2181	2
2754	2181	3
2755	2182	1
2756	2183	1
2757	2184	1
2758	2185	1
2759	2186	1
2760	2187	1
2761	2188	1
2762	2188	2
2763	2188	3
2768	2193	1
2769	2194	1
2770	2195	1
2771	2196	1
2772	2196	2
2773	2196	3
2774	2197	1
2775	2198	1
2776	2199	1
2777	2200	1
2778	2201	1
2779	2202	1
2780	2203	1
2781	2203	2
2782	2203	3
2787	2208	1
2788	2209	1
2789	2210	1
2790	2211	1
2791	2211	2
2792	2211	3
2793	2212	1
2794	2213	1
2795	2214	1
2796	2215	1
2797	2216	1
2798	2217	1
2799	2218	1
2800	2218	2
2801	2218	3
2806	2223	1
2807	2224	1
2808	2225	1
2809	2226	1
2810	2226	2
2811	2226	3
2812	2227	1
2813	2228	1
2814	2229	1
2815	2230	1
2816	2231	1
2817	2232	1
2818	2233	1
2819	2233	2
2820	2233	3
2825	2238	1
2826	2239	1
2827	2240	1
2828	2241	1
2829	2241	2
2830	2241	3
2831	2242	1
2832	2243	1
2833	2244	1
2834	2245	1
2835	2246	1
2836	2247	1
2837	2248	1
2838	2248	2
2839	2248	3
2844	2253	1
2845	2254	1
2846	2255	1
2847	2256	1
2848	2256	2
2849	2256	3
2850	2257	1
2851	2258	1
2852	2259	1
2853	2260	1
2854	2261	1
2855	2262	1
2856	2263	1
2857	2263	2
2858	2263	3
2863	2268	1
2864	2269	1
2865	2270	1
2866	2271	1
2867	2271	2
2868	2271	3
2869	2272	1
2870	2273	1
2871	2274	1
2872	2275	1
2873	2276	1
2874	2277	1
2875	2278	1
2876	2278	2
2877	2278	3
2882	2283	1
2883	2284	1
2884	2285	1
2885	2286	1
2886	2286	2
2887	2286	3
2888	2287	1
2889	2288	1
2890	2289	1
2891	2290	1
2892	2291	1
2893	2292	1
2894	2293	1
2895	2293	2
2896	2293	3
2901	2298	1
2902	2299	1
2903	2300	1
2904	2301	1
2905	2301	2
2906	2301	3
2907	2302	1
2908	2303	1
2909	2304	1
2910	2305	1
2911	2306	1
2912	2307	1
2913	2308	1
2914	2308	2
2915	2308	3
2920	2313	1
2921	2314	1
2922	2315	1
2923	2316	1
2924	2316	2
2925	2316	3
2926	2317	1
2927	2318	1
2928	2319	1
2929	2320	1
2930	2321	1
2931	2322	1
2932	2323	1
2933	2323	2
2934	2323	3
2939	2328	1
2940	2329	1
2941	2330	1
2942	2331	1
2943	2331	2
2944	2331	3
2945	2332	1
2946	2333	1
2947	2334	1
2948	2335	1
2949	2336	1
2950	2337	1
2951	2338	1
2952	2338	2
2953	2338	3
2958	2343	1
2959	2344	1
2960	2345	1
2961	2346	1
2962	2346	2
2963	2346	3
2964	2347	1
2965	2348	1
2966	2349	1
2967	2350	1
2968	2351	1
2969	2352	1
2970	2353	1
2971	2353	2
2972	2353	3
2977	2358	1
2978	2359	1
2979	2360	1
2980	2361	1
2981	2361	2
2982	2361	3
2983	2362	1
2984	2363	1
2985	2364	1
2986	2365	1
2987	2366	1
2988	2367	1
2989	2368	1
2990	2368	2
2991	2368	3
2996	2373	1
2997	2374	1
2998	2375	1
2999	2376	1
3000	2376	2
3001	2376	3
3002	2377	1
3003	2378	1
3004	2379	1
3005	2380	1
3006	2381	1
3007	2382	1
3008	2383	1
3009	2383	2
3010	2383	3
3015	2388	1
3016	2389	1
3017	2390	1
3018	2391	1
3019	2391	2
3020	2391	3
3021	2392	1
3022	2393	1
3023	2394	1
3024	2395	1
3025	2396	1
3026	2397	1
3027	2398	1
3028	2398	2
3029	2398	3
3034	2403	1
3035	2404	1
3036	2405	1
3037	2406	1
3038	2406	2
3039	2406	3
\.


--
-- TOC entry 4462 (class 0 OID 21185)
-- Dependencies: 212
-- Data for Name: FlagAttribute; Type: TABLE DATA; Schema: dbo; Owner: -
--

COPY dbo."FlagAttribute" ("FlagAttributeId", "Name") FROM stdin;
\.


--
-- TOC entry 4487 (class 0 OID 22366)
-- Dependencies: 237
-- Data for Name: FloatElement; Type: TABLE DATA; Schema: dbo; Owner: -
--

COPY dbo."FloatElement" ("FloatElementId", "Value") FROM stdin;
43	1.5983457893399999
62	1.5983457893399999
81	1.5983457893399999
91	1.5983457893399999
110	1.5983457893399999
119	1.5983457893399999
129	1.5983457893399999
148	1.5983457893399999
157	1.5983457893399999
167	1.5983457893399999
186	1.5983457893399999
195	1.5983457893399999
205	1.5983457893399999
224	1.5983457893399999
233	1.5983457893399999
243	1.5983457893399999
262	1.5983457893399999
271	1.5983457893399999
281	1.5983457893399999
300	1.5983457893399999
309	1.5983457893399999
319	1.5983457893399999
338	1.5983457893399999
347	1.5983457893399999
357	1.5983457893399999
376	1.5983457893399999
385	1.5983457893399999
395	1.5983457893399999
404	1.5983457893399999
414	1.5983457893399999
433	1.5983457893399999
442	1.5983457893399999
452	1.5983457893399999
461	1.5983457893399999
471	1.5983457893399999
490	1.5983457893399999
499	1.5983457893399999
509	1.5983457893399999
518	1.5983457893399999
528	1.5983457893399999
547	1.5983457893399999
556	1.5983457893399999
566	1.5983457893399999
575	1.5983457893399999
585	1.5983457893399999
604	1.5983457893399999
613	1.5983457893399999
623	1.5983457893399999
632	1.5983457893399999
642	1.5983457893399999
661	1.5983457893399999
670	1.5983457893399999
680	1.5983457893399999
689	1.5983457893399999
699	1.5983457893399999
718	1.5983457893399999
727	1.5983457893399999
737	1.5983457893399999
746	1.5983457893399999
756	1.5983457893399999
775	1.5983457893399999
784	1.5983457893399999
794	1.5983457893399999
803	1.5983457893399999
813	1.5983457893399999
832	1.5983457893399999
841	1.5983457893399999
851	1.5983457893399999
860	1.5983457893399999
870	1.5983457893399999
889	1.5983457893399999
898	1.5983457893399999
908	1.5983457893399999
917	1.5983457893399999
927	1.5983457893399999
946	1.5983457893399999
955	1.5983457893399999
965	1.5983457893399999
974	1.5983457893399999
984	1.5983457893399999
1003	1.5983457893399999
1012	1.5983457893399999
1022	1.5983457893399999
1031	1.5983457893399999
1041	1.5983457893399999
1060	1.5983457893399999
1069	1.5983457893399999
1079	1.5983457893399999
1088	1.5983457893399999
1098	1.5983457893399999
1117	1.5983457893399999
1126	1.5983457893399999
1136	1.5983457893399999
1145	1.5983457893399999
1155	1.5983457893399999
1174	1.5983457893399999
1183	1.5983457893399999
1193	1.5983457893399999
1202	1.5983457893399999
1212	1.5983457893399999
1231	1.5983457893399999
1240	1.5983457893399999
1250	1.5983457893399999
1259	1.5983457893399999
1269	1.5983457893399999
1288	1.5983457893399999
1297	1.5983457893399999
1307	1.5983457893399999
1316	1.5983457893399999
1326	1.5983457893399999
1345	1.5983457893399999
1354	1.5983457893399999
1364	1.5983457893399999
1373	1.5983457893399999
1383	1.5983457893399999
1402	1.5983457893399999
1411	1.5983457893399999
1421	1.5983457893399999
1430	1.5983457893399999
1440	1.5983457893399999
1459	1.5983457893399999
1468	1.5983457893399999
1478	1.5983457893399999
1487	1.5983457893399999
1497	1.5983457893399999
1516	1.5983457893399999
1525	1.5983457893399999
1535	1.5983457893399999
1544	1.5983457893399999
1554	1.5983457893399999
1573	1.5983457893399999
1582	1.5983457893399999
1592	1.5983457893399999
1601	1.5983457893399999
1611	1.5983457893399999
1630	1.5983457893399999
1639	1.5983457893399999
1649	1.5983457893399999
1658	1.5983457893399999
1668	1.5983457893399999
1687	1.5983457893399999
1696	1.5983457893399999
1706	1.5983457893399999
1715	1.5983457893399999
1725	1.5983457893399999
1744	1.5983457893399999
1753	1.5983457893399999
1763	1.5983457893399999
1772	1.5983457893399999
1782	1.5983457893399999
1801	1.5983457893399999
1810	1.5983457893399999
1820	1.5983457893399999
1829	1.5983457893399999
1839	1.5983457893399999
1858	1.5983457893399999
1867	1.5983457893399999
1877	1.5983457893399999
1886	1.5983457893399999
1896	1.5983457893399999
1915	1.5983457893399999
1924	1.5983457893399999
1934	1.5983457893399999
1943	1.5983457893399999
1953	1.5983457893399999
1972	1.5983457893399999
1981	1.5983457893399999
1991	1.5983457893399999
2000	1.5983457893399999
2010	1.5983457893399999
2029	1.5983457893399999
2038	1.5983457893399999
2048	1.5983457893399999
2057	1.5983457893399999
2067	1.5983457893399999
2086	1.5983457893399999
2095	1.5983457893399999
2105	1.5983457893399999
2114	1.5983457893399999
2124	1.5983457893399999
2143	1.5983457893399999
2152	1.5983457893399999
2162	1.5983457893399999
2171	1.5983457893399999
2181	1.5983457893399999
2200	1.5983457893399999
2209	1.5983457893399999
2219	1.5983457893399999
2228	1.5983457893399999
2238	1.5983457893399999
2257	1.5983457893399999
2266	1.5983457893399999
2276	1.5983457893399999
2285	1.5983457893399999
2295	1.5983457893399999
2314	1.5983457893399999
2323	1.5983457893399999
2333	1.5983457893399999
2342	1.5983457893399999
2352	1.5983457893399999
2371	1.5983457893399999
2380	1.5983457893399999
2390	1.5983457893399999
2399	1.5983457893399999
2409	1.5983457893399999
2428	1.5983457893399999
2437	1.5983457893399999
2447	1.5983457893399999
2456	1.5983457893399999
2466	1.5983457893399999
2485	1.5983457893399999
2494	1.5983457893399999
2504	1.5983457893399999
2513	1.5983457893399999
2523	1.5983457893399999
2542	1.5983457893399999
2551	1.5983457893399999
2561	1.5983457893399999
2570	1.5983457893399999
2580	1.5983457893399999
2599	1.5983457893399999
2608	1.5983457893399999
2618	1.5983457893399999
2627	1.5983457893399999
2637	1.5983457893399999
2656	1.5983457893399999
2665	1.5983457893399999
2675	1.5983457893399999
2684	1.5983457893399999
2694	1.5983457893399999
2713	1.5983457893399999
2722	1.5983457893399999
2732	1.5983457893399999
2741	1.5983457893399999
2751	1.5983457893399999
2770	1.5983457893399999
2779	1.5983457893399999
2789	1.5983457893399999
2798	1.5983457893399999
2808	1.5983457893399999
2827	1.5983457893399999
2836	1.5983457893399999
2846	1.5983457893399999
2855	1.5983457893399999
2865	1.5983457893399999
2884	1.5983457893399999
2893	1.5983457893399999
2903	1.5983457893399999
2912	1.5983457893399999
2922	1.5983457893399999
2941	1.5983457893399999
2950	1.5983457893399999
2960	1.5983457893399999
2969	1.5983457893399999
2979	1.5983457893399999
2998	1.5983457893399999
3007	1.5983457893399999
3017	1.5983457893399999
3026	1.5983457893399999
3036	1.5983457893399999
\.


--
-- TOC entry 4481 (class 0 OID 22235)
-- Dependencies: 231
-- Data for Name: GenericSubmission; Type: TABLE DATA; Schema: dbo; Owner: -
--

COPY dbo."GenericSubmission" ("GenericSubmissionId", "Subject", "SubmittedByDomainIdentifierId", "SubmittedTime") FROM stdin;
2	My Submission	13	2020-08-14 00:35:45.207961+00
3	My Submission	13	2020-08-14 00:46:14.30776+00
4	My Submission	13	2020-08-17 00:45:49.305+00
5	My Final MERGE Submission	50	2020-08-17 00:45:51.022+00
6	My Submission	13	2020-08-17 22:47:25.655+00
7	My Final MERGE Submission	50	2020-08-17 22:47:26.346+00
8	My Submission	13	2020-08-20 21:10:07.62+00
9	My Final MERGE Submission	50	2020-08-20 21:10:08.777+00
11	My Final MERGE Submission	50	2020-08-20 21:10:13.55+00
12	My Submission	13	2020-08-21 02:12:13.761+00
13	My Final MERGE Submission	50	2020-08-21 02:12:14.047+00
14	My Submission	13	2020-08-21 02:12:15.91+00
15	My Final MERGE Submission	50	2020-08-21 02:12:16.19+00
16	My Submission	13	2020-08-25 18:43:20.163+00
17	My Final MERGE Submission	50	2020-08-25 18:43:20.66+00
18	My Submission	13	2020-08-25 18:43:23.217+00
19	My Final MERGE Submission	50	2020-08-25 18:43:23.56+00
20	My Submission	13	2020-08-26 00:45:47.581+00
21	My Final MERGE Submission	50	2020-08-26 00:45:48.039+00
22	My Submission	13	2020-08-26 00:45:50.445+00
23	My Final MERGE Submission	50	2020-08-26 00:45:50.71+00
24	My Submission	13	2020-08-28 14:57:21.511+00
25	My Final MERGE Submission	50	2020-08-28 14:57:23.727+00
26	My Submission	13	2020-08-28 14:57:42.618+00
27	My Final MERGE Submission	50	2020-08-28 14:57:44.528+00
28	My Submission	13	2020-09-17 02:06:04.172+00
29	My Final MERGE Submission	50	2020-09-17 02:06:04.704+00
30	My Submission	13	2020-09-17 02:06:07.48+00
31	My Final MERGE Submission	50	2020-09-17 02:06:07.713+00
32	My Submission	13	2020-10-01 16:38:18.693+00
33	My Final MERGE Submission	50	2020-10-01 16:38:19.741+00
34	My Submission	13	2020-10-01 16:38:23.546+00
35	My Final MERGE Submission	50	2020-10-01 16:38:23.849+00
36	My Submission	13	2020-10-11 00:42:53.085+00
37	My Final MERGE Submission	50	2020-10-11 00:42:53.565+00
38	My Submission	13	2020-10-11 00:42:56.165+00
39	My Final MERGE Submission	50	2020-10-11 00:42:56.354+00
40	My Submission	13	2020-10-15 20:51:37.561+00
41	My Final MERGE Submission	50	2020-10-15 20:51:38.205+00
42	My Submission	13	2020-10-15 20:51:44.228+00
43	My Final MERGE Submission	50	2020-10-15 20:51:44.502+00
44	My Submission	13	2020-10-15 20:51:44.942+00
45	My Final MERGE Submission	50	2020-10-15 20:51:45.31+00
46	My Submission	13	2020-10-15 23:43:37.987+00
47	My Final MERGE Submission	50	2020-10-15 23:43:38.541+00
48	My Submission	13	2020-10-15 23:43:43.484+00
49	My Final MERGE Submission	50	2020-10-15 23:43:43.684+00
50	My Submission	13	2020-10-15 23:43:43.95+00
51	My Final MERGE Submission	50	2020-10-15 23:43:44.17+00
52	My Submission	13	2020-10-16 18:37:03.803+00
53	My Final MERGE Submission	50	2020-10-16 18:37:04.204+00
54	My Submission	13	2020-10-16 18:37:08.701+00
55	My Final MERGE Submission	50	2020-10-16 18:37:08.91+00
56	My Submission	13	2020-10-16 18:37:09.177+00
57	My Final MERGE Submission	50	2020-10-16 18:37:09.408+00
58	My Submission	13	2020-10-16 20:47:36.516+00
59	My Final MERGE Submission	50	2020-10-16 20:47:37.022+00
60	My Submission	13	2020-10-16 20:47:42.228+00
61	My Final MERGE Submission	50	2020-10-16 20:47:42.449+00
62	My Submission	13	2020-10-16 20:47:42.758+00
63	My Final MERGE Submission	50	2020-10-16 20:47:43.013+00
64	My Submission	13	2020-10-17 02:05:17.406+00
65	My Final MERGE Submission	50	2020-10-17 02:05:18.156+00
66	My Submission	13	2020-10-17 02:05:26.345+00
67	My Final MERGE Submission	50	2020-10-17 02:05:26.683+00
68	My Submission	13	2020-10-17 02:05:27.075+00
69	My Final MERGE Submission	50	2020-10-17 02:05:27.428+00
70	My Submission	13	2020-11-03 14:57:04.152+00
71	My Final MERGE Submission	50	2020-11-03 14:57:05.084+00
72	My Submission	13	2020-11-03 14:57:11.693+00
73	My Final MERGE Submission	50	2020-11-03 14:57:11.91+00
74	My Submission	13	2020-11-03 14:57:12.16+00
75	My Final MERGE Submission	50	2020-11-03 14:57:12.381+00
76	My Submission	13	2021-01-08 05:26:17.092+00
77	My Final MERGE Submission	50	2021-01-08 05:26:17.953+00
78	My Submission	13	2021-01-08 05:26:24.844+00
79	My Final MERGE Submission	50	2021-01-08 05:26:25.067+00
80	My Submission	13	2021-01-08 05:26:25.336+00
81	My Final MERGE Submission	50	2021-01-08 05:26:25.565+00
82	My Submission	13	2021-01-08 05:56:49.597+00
83	My Final MERGE Submission	50	2021-01-08 05:56:50.494+00
84	My Submission	13	2021-01-08 05:56:57.222+00
85	My Final MERGE Submission	50	2021-01-08 05:56:57.438+00
86	My Submission	13	2021-01-08 05:56:57.718+00
87	My Final MERGE Submission	50	2021-01-08 05:56:57.953+00
88	My Submission	13	2021-01-10 19:21:29.835+00
89	My MERGE Submission	13	2021-01-10 19:21:30.461+00
90	My Final MERGE Submission	50	2021-01-10 19:21:30.74+00
91	My Submission	13	2021-01-10 19:21:37.636+00
92	My MERGE Submission	13	2021-01-10 19:21:37.812+00
93	My Final MERGE Submission	50	2021-01-10 19:21:37.924+00
94	My Submission	13	2021-01-10 19:21:38.286+00
95	My MERGE Submission	13	2021-01-10 19:21:38.466+00
96	My Final MERGE Submission	50	2021-01-10 19:21:38.634+00
97	My Submission	13	2021-01-10 20:58:16.833+00
98	My MERGE Submission	13	2021-01-10 20:58:17.263+00
99	My Final MERGE Submission	50	2021-01-10 20:58:17.546+00
100	My Submission	13	2021-01-10 20:58:23.659+00
101	My MERGE Submission	13	2021-01-10 20:58:23.785+00
102	My Final MERGE Submission	50	2021-01-10 20:58:23.871+00
103	My Submission	13	2021-01-10 20:58:24.131+00
104	My MERGE Submission	13	2021-01-10 20:58:24.268+00
105	My Final MERGE Submission	50	2021-01-10 20:58:24.367+00
106	My Submission	13	2021-01-12 16:45:39.69+00
107	My MERGE Submission	13	2021-01-12 16:45:40.579+00
108	My Final MERGE Submission	50	2021-01-12 16:45:41.018+00
109	My Submission	13	2021-01-12 16:45:48.066+00
110	My MERGE Submission	13	2021-01-12 16:45:48.246+00
111	My Final MERGE Submission	50	2021-01-12 16:45:48.376+00
112	My Submission	13	2021-01-12 16:45:48.682+00
113	My MERGE Submission	13	2021-01-12 16:45:48.878+00
114	My Final MERGE Submission	50	2021-01-12 16:45:49.018+00
115	My Submission	13	2021-01-13 19:21:57.67+00
116	My MERGE Submission	13	2021-01-13 19:21:57.982+00
117	My Final MERGE Submission	50	2021-01-13 19:21:58.182+00
118	My Submission	13	2021-01-13 19:22:02.417+00
119	My MERGE Submission	13	2021-01-13 19:22:02.59+00
120	My Final MERGE Submission	50	2021-01-13 19:22:02.664+00
121	My Submission	13	2021-01-13 19:22:03.029+00
122	My MERGE Submission	13	2021-01-13 19:22:03.226+00
123	My Final MERGE Submission	50	2021-01-13 19:22:03.311+00
124	My Submission	13	2021-01-14 04:28:00.506+00
125	My MERGE Submission	13	2021-01-14 04:28:01.006+00
126	My Final MERGE Submission	50	2021-01-14 04:28:01.235+00
127	My Submission	13	2021-01-14 04:28:07.928+00
128	My MERGE Submission	13	2021-01-14 04:28:08.139+00
129	My Final MERGE Submission	50	2021-01-14 04:28:08.278+00
130	My Submission	13	2021-01-14 04:28:08.672+00
131	My MERGE Submission	13	2021-01-14 04:28:08.903+00
132	My Final MERGE Submission	50	2021-01-14 04:28:09.052+00
133	My Submission	13	2021-01-21 18:55:59.007+00
134	My MERGE Submission	13	2021-01-21 18:56:00.322+00
135	My Final MERGE Submission	50	2021-01-21 18:56:01.306+00
136	My Submission	13	2021-01-21 18:56:41.575+00
137	My MERGE Submission	13	2021-01-21 18:56:42.555+00
138	My Final MERGE Submission	50	2021-01-21 18:56:43.381+00
139	My Submission	13	2021-01-21 18:56:45.07+00
140	My MERGE Submission	13	2021-01-21 18:56:46.041+00
141	My Final MERGE Submission	50	2021-01-21 18:56:46.841+00
142	My Submission	13	2021-01-22 20:34:40.554+00
143	My MERGE Submission	13	2021-01-22 20:34:40.977+00
144	My Final MERGE Submission	50	2021-01-22 20:34:41.221+00
145	My Submission	13	2021-01-22 20:34:49.294+00
146	My MERGE Submission	13	2021-01-22 20:34:49.565+00
147	My Final MERGE Submission	50	2021-01-22 20:34:49.727+00
148	My Submission	13	2021-01-22 20:34:50.217+00
149	My MERGE Submission	13	2021-01-22 20:34:50.495+00
150	My Final MERGE Submission	50	2021-01-22 20:34:50.662+00
151	My Submission	13	2021-01-25 15:12:17.204+00
152	My MERGE Submission	13	2021-01-25 15:12:17.428+00
153	My Final MERGE Submission	50	2021-01-25 15:12:17.534+00
154	My Submission	13	2021-01-25 15:12:21.264+00
155	My MERGE Submission	13	2021-01-25 15:12:21.368+00
156	My Final MERGE Submission	50	2021-01-25 15:12:21.461+00
157	My Submission	13	2021-01-25 15:12:21.695+00
158	My MERGE Submission	13	2021-01-25 15:12:21.807+00
159	My Final MERGE Submission	50	2021-01-25 15:12:21.894+00
160	My Submission	13	2021-01-28 19:07:22.778+00
161	My MERGE Submission	13	2021-01-28 19:07:23.968+00
162	My Final MERGE Submission	50	2021-01-28 19:07:24.805+00
163	My Submission	13	2021-01-28 19:08:04.044+00
164	My MERGE Submission	13	2021-01-28 19:08:05.092+00
165	My Final MERGE Submission	50	2021-01-28 19:08:05.852+00
166	My Submission	13	2021-01-28 19:08:07.392+00
167	My MERGE Submission	13	2021-01-28 19:08:08.309+00
168	My Final MERGE Submission	50	2021-01-28 19:08:09.071+00
169	My Submission	13	2021-02-05 04:40:32.098+00
170	My MERGE Submission	13	2021-02-05 04:40:33.315+00
171	My Final MERGE Submission	50	2021-02-05 04:40:34.232+00
172	My Submission	13	2021-02-05 04:41:16.665+00
173	My MERGE Submission	13	2021-02-05 04:41:17.662+00
174	My Final MERGE Submission	50	2021-02-05 04:41:18.492+00
175	My Submission	13	2021-02-05 04:41:20.152+00
176	My MERGE Submission	13	2021-02-05 04:41:21.151+00
177	My Final MERGE Submission	50	2021-02-05 04:41:21.998+00
178	My Submission	13	2021-02-06 04:44:40.416+00
179	My MERGE Submission	13	2021-02-06 04:44:41.654+00
180	My Final MERGE Submission	50	2021-02-06 04:44:42.543+00
181	My Submission	13	2021-02-06 04:45:24.621+00
182	My MERGE Submission	13	2021-02-06 04:45:25.679+00
183	My Final MERGE Submission	50	2021-02-06 04:45:26.535+00
184	My Submission	13	2021-02-06 04:45:28.294+00
185	My MERGE Submission	13	2021-02-06 04:45:29.336+00
186	My Final MERGE Submission	50	2021-02-06 04:45:30.201+00
187	My Submission	705	2021-02-09 15:48:41.373+00
188	My MERGE Submission	705	2021-02-09 15:48:42.701+00
189	My Final MERGE Submission	706	2021-02-09 15:48:43.637+00
190	My Submission	705	2021-02-09 15:49:28.839+00
191	My MERGE Submission	705	2021-02-09 15:49:29.909+00
192	My Final MERGE Submission	706	2021-02-09 15:49:30.797+00
193	My Submission	705	2021-02-09 15:49:32.583+00
194	My MERGE Submission	705	2021-02-09 15:49:33.649+00
195	My Final MERGE Submission	706	2021-02-09 15:49:34.542+00
196	My Submission	705	2021-02-09 15:51:13.315+00
197	My MERGE Submission	705	2021-02-09 15:51:14.563+00
198	My Final MERGE Submission	706	2021-02-09 15:51:15.516+00
199	My Submission	705	2021-02-09 15:52:01.274+00
200	My MERGE Submission	705	2021-02-09 15:52:02.343+00
201	My Final MERGE Submission	706	2021-02-09 15:52:03.233+00
202	My Submission	705	2021-02-09 15:52:04.99+00
203	My MERGE Submission	705	2021-02-09 15:52:06.072+00
204	My Final MERGE Submission	706	2021-02-09 15:52:06.959+00
205	My Submission	705	2021-02-09 15:53:51.555+00
206	My MERGE Submission	705	2021-02-09 15:53:52.835+00
207	My Final MERGE Submission	706	2021-02-09 15:53:53.813+00
208	My Submission	705	2021-02-09 15:54:39.556+00
209	My MERGE Submission	705	2021-02-09 15:54:40.663+00
210	My Final MERGE Submission	706	2021-02-09 15:54:41.549+00
211	My Submission	705	2021-02-09 15:54:43.33+00
212	My MERGE Submission	705	2021-02-09 15:54:44.413+00
213	My Final MERGE Submission	706	2021-02-09 15:54:45.307+00
214	My Submission	705	2021-02-09 15:56:22.286+00
215	My MERGE Submission	705	2021-02-09 15:56:23.565+00
216	My Final MERGE Submission	706	2021-02-09 15:56:24.547+00
217	My Submission	705	2021-02-09 15:57:10.099+00
218	My MERGE Submission	705	2021-02-09 15:57:11.174+00
219	My Final MERGE Submission	706	2021-02-09 15:57:12.059+00
220	My Submission	705	2021-02-09 15:57:13.807+00
221	My MERGE Submission	705	2021-02-09 15:57:14.861+00
222	My Final MERGE Submission	706	2021-02-09 15:57:15.749+00
223	My Submission	705	2021-02-09 16:05:51.283+00
224	My MERGE Submission	705	2021-02-09 16:05:51.845+00
225	My Final MERGE Submission	706	2021-02-09 16:05:52.31+00
226	My Submission	705	2021-02-09 16:06:10.149+00
227	My MERGE Submission	705	2021-02-09 16:06:10.583+00
228	My Final MERGE Submission	706	2021-02-09 16:06:10.924+00
229	My Submission	705	2021-02-09 16:06:11.634+00
230	My MERGE Submission	705	2021-02-09 16:06:12.06+00
231	My Final MERGE Submission	706	2021-02-09 16:06:12.398+00
232	My Submission	705	2021-02-09 16:07:05.292+00
233	My MERGE Submission	705	2021-02-09 16:07:05.943+00
234	My Final MERGE Submission	706	2021-02-09 16:07:06.373+00
235	My Submission	705	2021-02-09 16:07:24.143+00
236	My MERGE Submission	705	2021-02-09 16:07:24.558+00
237	My Final MERGE Submission	706	2021-02-09 16:07:24.887+00
238	My Submission	705	2021-02-09 16:07:25.576+00
239	My MERGE Submission	705	2021-02-09 16:07:25.979+00
240	My Final MERGE Submission	706	2021-02-09 16:07:26.317+00
241	My Submission	705	2021-02-09 16:08:24.412+00
242	My MERGE Submission	705	2021-02-09 16:08:25.082+00
243	My Final MERGE Submission	706	2021-02-09 16:08:25.516+00
244	My Submission	705	2021-02-09 16:08:42.783+00
245	My MERGE Submission	705	2021-02-09 16:08:43.203+00
246	My Final MERGE Submission	706	2021-02-09 16:08:43.537+00
247	My Submission	705	2021-02-09 16:08:44.239+00
248	My MERGE Submission	705	2021-02-09 16:08:44.655+00
249	My Final MERGE Submission	706	2021-02-09 16:08:44.991+00
250	My Submission	705	2021-02-09 16:09:37.368+00
251	My MERGE Submission	705	2021-02-09 16:09:38.014+00
252	My Final MERGE Submission	706	2021-02-09 16:09:38.461+00
253	My Submission	705	2021-02-09 16:09:56.665+00
254	My MERGE Submission	705	2021-02-09 16:09:57.097+00
255	My Final MERGE Submission	706	2021-02-09 16:09:57.428+00
256	My Submission	705	2021-02-09 16:09:58.134+00
257	My MERGE Submission	705	2021-02-09 16:09:58.566+00
258	My Final MERGE Submission	706	2021-02-09 16:09:58.905+00
259	My Submission	705	2021-02-09 19:44:03.121+00
260	My MERGE Submission	705	2021-02-09 19:44:03.602+00
261	My Final MERGE Submission	706	2021-02-09 19:44:03.925+00
262	My Submission	705	2021-02-09 19:44:18.325+00
263	My MERGE Submission	705	2021-02-09 19:44:18.711+00
264	My Final MERGE Submission	706	2021-02-09 19:44:18.994+00
265	My Submission	705	2021-02-09 19:44:19.6+00
266	My MERGE Submission	705	2021-02-09 19:44:19.961+00
267	My Final MERGE Submission	706	2021-02-09 19:44:20.25+00
268	My Submission	705	2021-02-09 19:45:12.883+00
269	My MERGE Submission	705	2021-02-09 19:45:13.433+00
270	My Final MERGE Submission	706	2021-02-09 19:45:13.808+00
271	My Submission	705	2021-02-09 19:45:29.267+00
272	My MERGE Submission	705	2021-02-09 19:45:29.725+00
273	My Final MERGE Submission	706	2021-02-09 19:45:30.018+00
274	My Submission	705	2021-02-09 19:45:30.626+00
275	My MERGE Submission	705	2021-02-09 19:45:30.976+00
276	My Final MERGE Submission	706	2021-02-09 19:45:31.253+00
277	My Submission	705	2021-02-09 19:46:28.657+00
278	My MERGE Submission	705	2021-02-09 19:46:29.249+00
279	My Final MERGE Submission	706	2021-02-09 19:46:29.621+00
280	My Submission	705	2021-02-09 19:46:44.814+00
281	My MERGE Submission	705	2021-02-09 19:46:45.231+00
282	My Final MERGE Submission	706	2021-02-09 19:46:45.51+00
283	My Submission	705	2021-02-09 19:46:46.137+00
284	My MERGE Submission	705	2021-02-09 19:46:46.491+00
285	My Final MERGE Submission	706	2021-02-09 19:46:46.779+00
286	My Submission	705	2021-02-09 19:47:38.698+00
287	My MERGE Submission	705	2021-02-09 19:47:39.251+00
288	My Final MERGE Submission	706	2021-02-09 19:47:39.619+00
289	My Submission	705	2021-02-09 19:47:54.599+00
290	My MERGE Submission	705	2021-02-09 19:47:54.963+00
291	My Final MERGE Submission	706	2021-02-09 19:47:55.239+00
292	My Submission	705	2021-02-09 19:47:55.841+00
293	My MERGE Submission	705	2021-02-09 19:47:56.197+00
294	My Final MERGE Submission	706	2021-02-09 19:47:56.498+00
295	My Submission	705	2021-02-09 20:50:55.753+00
296	My MERGE Submission	705	2021-02-09 20:50:56.448+00
297	My Final MERGE Submission	706	2021-02-09 20:50:56.903+00
298	My Submission	705	2021-02-09 20:51:12.723+00
299	My MERGE Submission	705	2021-02-09 20:51:13.102+00
300	My Final MERGE Submission	706	2021-02-09 20:51:13.418+00
301	My Submission	705	2021-02-09 20:51:14.054+00
302	My MERGE Submission	705	2021-02-09 20:51:14.449+00
303	My Final MERGE Submission	706	2021-02-09 20:51:14.75+00
304	My Submission	705	2021-02-09 20:51:59.205+00
305	My MERGE Submission	705	2021-02-09 20:51:59.739+00
306	My Final MERGE Submission	706	2021-02-09 20:52:00.138+00
307	My Submission	705	2021-02-09 20:52:16.494+00
308	My MERGE Submission	705	2021-02-09 20:52:16.874+00
309	My Final MERGE Submission	706	2021-02-09 20:52:17.173+00
310	My Submission	705	2021-02-09 20:52:17.789+00
311	My MERGE Submission	705	2021-02-09 20:52:18.154+00
312	My Final MERGE Submission	706	2021-02-09 20:52:18.453+00
313	My Submission	705	2021-02-09 20:53:08.598+00
314	My MERGE Submission	705	2021-02-09 20:53:09.166+00
315	My Final MERGE Submission	706	2021-02-09 20:53:09.596+00
316	My Submission	705	2021-02-09 20:53:25.545+00
317	My MERGE Submission	705	2021-02-09 20:53:25.921+00
318	My Final MERGE Submission	706	2021-02-09 20:53:26.22+00
319	My Submission	705	2021-02-09 20:53:26.851+00
320	My MERGE Submission	705	2021-02-09 20:53:27.237+00
321	My Final MERGE Submission	706	2021-02-09 20:53:27.545+00
322	My Submission	705	2021-02-09 20:54:13.475+00
323	My MERGE Submission	705	2021-02-09 20:54:14.041+00
324	My Final MERGE Submission	706	2021-02-09 20:54:14.455+00
325	My Submission	705	2021-02-09 20:54:30.475+00
326	My MERGE Submission	705	2021-02-09 20:54:30.854+00
327	My Final MERGE Submission	706	2021-02-09 20:54:31.153+00
328	My Submission	705	2021-02-09 20:54:31.772+00
329	My MERGE Submission	705	2021-02-09 20:54:32.14+00
330	My Final MERGE Submission	706	2021-02-09 20:54:32.439+00
331	My Submission	705	2021-02-09 21:35:04.107+00
332	My MERGE Submission	705	2021-02-09 21:35:04.418+00
333	My Final MERGE Submission	706	2021-02-09 21:35:04.651+00
334	My Submission	705	2021-02-09 21:35:09.774+00
335	My MERGE Submission	705	2021-02-09 21:35:09.877+00
336	My Final MERGE Submission	706	2021-02-09 21:35:09.947+00
337	My Submission	705	2021-02-09 21:35:10.178+00
338	My MERGE Submission	705	2021-02-09 21:35:10.297+00
339	My Final MERGE Submission	706	2021-02-09 21:35:10.377+00
340	My Submission	705	2021-02-09 21:35:39.747+00
341	My MERGE Submission	705	2021-02-09 21:35:40.051+00
342	My Final MERGE Submission	706	2021-02-09 21:35:40.222+00
343	My Submission	705	2021-02-09 21:35:45.372+00
344	My MERGE Submission	705	2021-02-09 21:35:45.47+00
345	My Final MERGE Submission	706	2021-02-09 21:35:45.551+00
346	My Submission	705	2021-02-09 21:35:45.767+00
347	My MERGE Submission	705	2021-02-09 21:35:45.874+00
348	My Final MERGE Submission	706	2021-02-09 21:35:45.973+00
349	My Submission	705	2021-02-09 21:36:22.999+00
350	My MERGE Submission	705	2021-02-09 21:36:23.335+00
351	My Final MERGE Submission	706	2021-02-09 21:36:23.527+00
352	My Submission	705	2021-02-09 21:36:28.554+00
353	My MERGE Submission	705	2021-02-09 21:36:28.679+00
354	My Final MERGE Submission	706	2021-02-09 21:36:28.754+00
355	My Submission	705	2021-02-09 21:36:28.99+00
356	My MERGE Submission	705	2021-02-09 21:36:29.117+00
357	My Final MERGE Submission	706	2021-02-09 21:36:29.202+00
358	My Submission	705	2021-02-09 21:36:58.306+00
359	My MERGE Submission	705	2021-02-09 21:36:58.632+00
360	My Final MERGE Submission	706	2021-02-09 21:36:58.825+00
361	My Submission	705	2021-02-09 21:37:03.921+00
362	My MERGE Submission	705	2021-02-09 21:37:04.024+00
363	My Final MERGE Submission	706	2021-02-09 21:37:04.094+00
364	My Submission	705	2021-02-09 21:37:04.292+00
365	My MERGE Submission	705	2021-02-09 21:37:04.404+00
366	My Final MERGE Submission	706	2021-02-09 21:37:04.519+00
367	My Submission	705	2021-02-09 21:59:07.088+00
368	My MERGE Submission	705	2021-02-09 21:59:07.465+00
369	My Final MERGE Submission	706	2021-02-09 21:59:07.726+00
370	My Submission	705	2021-02-09 21:59:14.866+00
371	My MERGE Submission	705	2021-02-09 21:59:15.025+00
372	My Final MERGE Submission	706	2021-02-09 21:59:15.143+00
373	My Submission	705	2021-02-09 21:59:15.455+00
374	My MERGE Submission	705	2021-02-09 21:59:15.62+00
375	My Final MERGE Submission	706	2021-02-09 21:59:15.747+00
376	My Submission	705	2021-02-09 21:59:38.619+00
377	My MERGE Submission	705	2021-02-09 21:59:38.881+00
378	My Final MERGE Submission	706	2021-02-09 21:59:39.042+00
379	My Submission	705	2021-02-09 21:59:46.044+00
380	My MERGE Submission	705	2021-02-09 21:59:46.251+00
381	My Final MERGE Submission	706	2021-02-09 21:59:46.363+00
382	My Submission	705	2021-02-09 21:59:46.66+00
383	My MERGE Submission	705	2021-02-09 21:59:46.814+00
384	My Final MERGE Submission	706	2021-02-09 21:59:46.93+00
385	My Submission	705	2021-02-09 22:00:15.961+00
386	My MERGE Submission	705	2021-02-09 22:00:16.296+00
387	My Final MERGE Submission	706	2021-02-09 22:00:16.529+00
388	My Submission	705	2021-02-09 22:00:23.643+00
389	My MERGE Submission	705	2021-02-09 22:00:23.816+00
390	My Final MERGE Submission	706	2021-02-09 22:00:23.934+00
391	My Submission	705	2021-02-09 22:00:24.249+00
392	My MERGE Submission	705	2021-02-09 22:00:24.419+00
393	My Final MERGE Submission	706	2021-02-09 22:00:24.543+00
394	My Submission	705	2021-02-09 22:00:46.431+00
395	My MERGE Submission	705	2021-02-09 22:00:46.779+00
396	My Final MERGE Submission	706	2021-02-09 22:00:46.981+00
397	My Submission	705	2021-02-09 22:00:53.876+00
398	My MERGE Submission	705	2021-02-09 22:00:54.022+00
399	My Final MERGE Submission	706	2021-02-09 22:00:54.136+00
400	My Submission	705	2021-02-09 22:00:54.419+00
401	My MERGE Submission	705	2021-02-09 22:00:54.601+00
402	My Final MERGE Submission	706	2021-02-09 22:00:54.726+00
403	My Submission	705	2021-02-10 00:17:00.351+00
404	My MERGE Submission	705	2021-02-10 00:17:01.547+00
405	My Final MERGE Submission	706	2021-02-10 00:17:02.461+00
406	My Submission	705	2021-02-10 00:17:42.542+00
407	My MERGE Submission	705	2021-02-10 00:17:43.471+00
408	My Final MERGE Submission	706	2021-02-10 00:17:44.251+00
409	My Submission	705	2021-02-10 00:17:45.865+00
410	My MERGE Submission	705	2021-02-10 00:17:46.827+00
411	My Final MERGE Submission	706	2021-02-10 00:17:47.615+00
412	My Submission	705	2021-02-10 00:19:25.029+00
413	My MERGE Submission	705	2021-02-10 00:19:26.215+00
414	My Final MERGE Submission	706	2021-02-10 00:19:27.112+00
415	My Submission	705	2021-02-10 00:20:07.382+00
416	My MERGE Submission	705	2021-02-10 00:20:08.365+00
417	My Final MERGE Submission	706	2021-02-10 00:20:09.164+00
418	My Submission	705	2021-02-10 00:20:10.707+00
419	My MERGE Submission	705	2021-02-10 00:20:11.639+00
420	My Final MERGE Submission	706	2021-02-10 00:20:12.427+00
421	My Submission	705	2021-02-10 00:21:52.062+00
422	My MERGE Submission	705	2021-02-10 00:21:53.24+00
423	My Final MERGE Submission	706	2021-02-10 00:21:54.254+00
424	My Submission	705	2021-02-10 00:22:36.139+00
425	My MERGE Submission	705	2021-02-10 00:22:37.112+00
426	My Final MERGE Submission	706	2021-02-10 00:22:37.934+00
427	My Submission	705	2021-02-10 00:22:39.579+00
428	My MERGE Submission	705	2021-02-10 00:22:40.556+00
429	My Final MERGE Submission	706	2021-02-10 00:22:41.371+00
430	My Submission	705	2021-02-10 00:24:13.739+00
431	My MERGE Submission	705	2021-02-10 00:24:14.826+00
432	My Final MERGE Submission	706	2021-02-10 00:24:15.686+00
433	My Submission	705	2021-02-10 00:24:56.869+00
434	My MERGE Submission	705	2021-02-10 00:24:57.828+00
435	My Final MERGE Submission	706	2021-02-10 00:24:58.636+00
436	My Submission	705	2021-02-10 00:25:00.255+00
437	My MERGE Submission	705	2021-02-10 00:25:01.22+00
438	My Final MERGE Submission	706	2021-02-10 00:25:02.04+00
\.


--
-- TOC entry 4482 (class 0 OID 22249)
-- Dependencies: 232
-- Data for Name: GenericSubmissionValue; Type: TABLE DATA; Schema: dbo; Owner: -
--

COPY dbo."GenericSubmissionValue" ("GenericSubmissionValueId", "GenericSubmissionId") FROM stdin;
8	2
9	2
10	2
11	2
12	2
13	2
14	2
15	3
16	3
17	3
18	3
19	3
20	3
21	3
22	4
23	4
24	4
25	4
26	4
27	4
28	4
33	5
34	5
35	5
36	5
37	6
38	6
39	6
40	6
41	6
42	6
43	6
48	7
49	7
50	7
51	7
52	8
53	8
54	8
55	8
56	8
57	8
58	8
63	9
64	9
65	9
66	9
78	11
79	11
80	11
81	11
82	12
83	12
84	12
85	12
86	12
87	12
88	12
93	13
94	13
95	13
96	13
97	14
98	14
99	14
100	14
101	14
102	14
103	14
108	15
109	15
110	15
111	15
112	16
113	16
114	16
115	16
116	16
117	16
118	16
123	17
124	17
125	17
126	17
127	18
128	18
129	18
130	18
131	18
132	18
133	18
138	19
139	19
140	19
141	19
142	20
143	20
144	20
145	20
146	20
147	20
148	20
153	21
154	21
155	21
156	21
157	22
158	22
159	22
160	22
161	22
162	22
163	22
168	23
169	23
170	23
171	23
172	24
173	24
174	24
175	24
176	24
177	24
178	24
183	25
184	25
185	25
186	25
187	26
188	26
189	26
190	26
191	26
192	26
193	26
198	27
199	27
200	27
201	27
202	28
203	28
204	28
205	28
206	28
207	28
208	28
213	29
214	29
215	29
216	29
217	30
218	30
219	30
220	30
221	30
222	30
223	30
228	31
229	31
230	31
231	31
232	32
233	32
234	32
235	32
236	32
237	32
238	32
243	33
244	33
245	33
246	33
247	34
248	34
249	34
250	34
251	34
252	34
253	34
382	52
383	52
384	52
258	35
259	35
260	35
261	35
262	36
263	36
264	36
265	36
266	36
267	36
268	36
385	52
386	52
387	52
388	52
273	37
274	37
275	37
276	37
277	38
278	38
279	38
280	38
281	38
282	38
283	38
1147	187
1148	187
1149	187
1150	187
288	39
289	39
290	39
291	39
292	40
293	40
294	40
295	40
296	40
297	40
298	40
393	53
394	53
395	53
396	53
303	41
304	41
305	41
306	41
307	42
308	42
309	42
310	42
311	42
312	42
313	42
397	54
398	54
399	54
400	54
318	43
319	43
320	43
321	43
322	44
323	44
324	44
325	44
326	44
327	44
328	44
401	54
402	54
403	54
333	45
334	45
335	45
336	45
337	46
338	46
339	46
340	46
341	46
342	46
343	46
408	55
348	47
349	47
350	47
351	47
352	48
353	48
354	48
355	48
356	48
357	48
358	48
409	55
410	55
411	55
412	56
363	49
364	49
365	49
366	49
367	50
368	50
369	50
370	50
371	50
372	50
373	50
413	56
414	56
415	56
416	56
378	51
379	51
380	51
381	51
417	56
418	56
423	57
424	57
425	57
426	57
427	58
428	58
429	58
430	58
431	58
432	58
433	58
438	59
439	59
440	59
441	59
442	60
443	60
444	60
445	60
446	60
447	60
448	60
453	61
454	61
455	61
456	61
457	62
458	62
459	62
460	62
461	62
462	62
463	62
468	63
469	63
470	63
471	63
472	64
473	64
474	64
475	64
476	64
477	64
478	64
483	65
484	65
485	65
486	65
487	66
488	66
489	66
490	66
491	66
492	66
493	66
498	67
499	67
500	67
501	67
502	68
503	68
504	68
505	68
506	68
507	68
508	68
1151	187
1152	187
1153	187
513	69
514	69
515	69
516	69
517	70
518	70
519	70
520	70
521	70
522	70
523	70
528	71
529	71
530	71
531	71
532	72
533	72
534	72
535	72
536	72
537	72
538	72
543	73
544	73
545	73
546	73
547	74
548	74
549	74
550	74
551	74
552	74
553	74
558	75
559	75
560	75
561	75
562	76
563	76
564	76
565	76
566	76
567	76
568	76
573	77
574	77
575	77
576	77
577	78
578	78
579	78
580	78
581	78
582	78
583	78
588	79
589	79
590	79
591	79
592	80
593	80
594	80
595	80
596	80
597	80
598	80
603	81
604	81
605	81
606	81
607	82
608	82
609	82
610	82
611	82
612	82
613	82
618	83
619	83
620	83
621	83
622	84
623	84
624	84
625	84
626	84
627	84
628	84
633	85
634	85
635	85
636	85
637	86
638	86
639	86
640	86
641	86
642	86
643	86
648	87
649	87
650	87
651	87
652	88
653	88
654	88
655	88
656	88
657	88
658	88
663	90
664	90
665	90
666	90
667	91
668	91
669	91
670	91
671	91
672	91
673	91
678	93
679	93
680	93
681	93
682	94
683	94
684	94
685	94
686	94
687	94
688	94
693	96
694	96
695	96
696	96
697	97
698	97
699	97
700	97
701	97
702	97
703	97
708	99
709	99
710	99
711	99
712	100
713	100
714	100
715	100
716	100
717	100
718	100
723	102
724	102
725	102
726	102
727	103
728	103
729	103
730	103
731	103
732	103
733	103
738	105
739	105
740	105
741	105
742	106
743	106
744	106
745	106
746	106
747	106
748	106
1158	189
753	108
754	108
755	108
756	108
757	109
758	109
759	109
760	109
761	109
762	109
763	109
768	111
769	111
770	111
771	111
772	112
773	112
774	112
775	112
776	112
777	112
778	112
783	114
784	114
785	114
786	114
787	115
788	115
789	115
790	115
791	115
792	115
793	115
798	117
799	117
800	117
801	117
802	118
803	118
804	118
805	118
806	118
807	118
808	118
813	120
814	120
815	120
816	120
817	121
818	121
819	121
820	121
821	121
822	121
823	121
828	123
829	123
830	123
831	123
832	124
833	124
834	124
835	124
836	124
837	124
838	124
843	126
844	126
845	126
846	126
847	127
848	127
849	127
850	127
851	127
852	127
853	127
858	129
859	129
860	129
861	129
862	130
863	130
864	130
865	130
866	130
867	130
868	130
873	132
874	132
875	132
876	132
877	133
878	133
879	133
880	133
881	133
882	133
883	133
888	135
889	135
890	135
891	135
892	136
893	136
894	136
895	136
896	136
897	136
898	136
903	138
904	138
905	138
906	138
907	139
908	139
909	139
910	139
911	139
912	139
913	139
918	141
919	141
920	141
921	141
922	142
923	142
924	142
925	142
926	142
927	142
928	142
933	144
934	144
935	144
936	144
937	145
938	145
939	145
940	145
941	145
942	145
943	145
948	147
949	147
950	147
951	147
952	148
953	148
954	148
955	148
956	148
957	148
958	148
963	150
964	150
965	150
966	150
967	151
968	151
969	151
970	151
971	151
972	151
973	151
978	153
979	153
980	153
981	153
982	154
983	154
984	154
985	154
986	154
987	154
988	154
1159	189
1160	189
1161	189
1162	190
993	156
994	156
995	156
996	156
997	157
998	157
999	157
1000	157
1001	157
1002	157
1003	157
1163	190
1164	190
1165	190
1166	190
1008	159
1009	159
1010	159
1011	159
1012	160
1013	160
1014	160
1015	160
1016	160
1017	160
1018	160
1167	190
1168	190
1023	162
1024	162
1025	162
1026	162
1027	163
1028	163
1029	163
1030	163
1031	163
1032	163
1033	163
1173	192
1174	192
1038	165
1039	165
1040	165
1041	165
1042	166
1043	166
1044	166
1045	166
1046	166
1047	166
1048	166
1175	192
1176	192
1177	193
1178	193
1053	168
1054	168
1055	168
1056	168
1057	169
1058	169
1059	169
1060	169
1061	169
1062	169
1063	169
1179	193
1180	193
1181	193
1182	193
1068	171
1069	171
1070	171
1071	171
1072	172
1073	172
1074	172
1075	172
1076	172
1077	172
1078	172
1183	193
1083	174
1084	174
1085	174
1086	174
1087	175
1088	175
1089	175
1090	175
1091	175
1092	175
1093	175
1188	195
1189	195
1190	195
1098	177
1099	177
1100	177
1101	177
1102	178
1103	178
1104	178
1105	178
1106	178
1107	178
1108	178
1191	195
1192	196
1193	196
1194	196
1113	180
1114	180
1115	180
1116	180
1117	181
1118	181
1119	181
1120	181
1121	181
1122	181
1123	181
1195	196
1196	196
1197	196
1198	196
1128	183
1129	183
1130	183
1131	183
1132	184
1133	184
1134	184
1135	184
1136	184
1137	184
1138	184
1143	186
1144	186
1145	186
1146	186
1203	198
1204	198
1205	198
1206	198
1207	199
1208	199
1209	199
1210	199
1211	199
1212	199
1213	199
1218	201
1219	201
1220	201
1221	201
1222	202
1223	202
1224	202
1225	202
1226	202
1227	202
1228	202
1233	204
1234	204
1235	204
1236	204
1237	205
1238	205
1239	205
1240	205
1241	205
1242	205
1243	205
1248	207
1249	207
1250	207
1251	207
1252	208
1253	208
1254	208
1255	208
1256	208
1257	208
1258	208
1263	210
1264	210
1265	210
1266	210
1267	211
1268	211
1269	211
1270	211
1271	211
1272	211
1273	211
1278	213
1279	213
1280	213
1281	213
1282	214
1283	214
1284	214
1285	214
1286	214
1287	214
1288	214
1293	216
1294	216
1295	216
1296	216
1297	217
1298	217
1299	217
1300	217
1301	217
1302	217
1303	217
1308	219
1309	219
1310	219
1311	219
1312	220
1313	220
1314	220
1315	220
1316	220
1317	220
1318	220
1323	222
1324	222
1325	222
1326	222
1327	223
1328	223
1329	223
1330	223
1331	223
1332	223
1333	223
1338	225
1339	225
1340	225
1341	225
1342	226
1343	226
1344	226
1345	226
1346	226
1347	226
1348	226
1353	228
1354	228
1355	228
1356	228
1357	229
1358	229
1359	229
1360	229
1361	229
1362	229
1363	229
1368	231
1369	231
1370	231
1371	231
1372	232
1373	232
1374	232
1375	232
1376	232
1377	232
1378	232
1383	234
1384	234
1385	234
1386	234
1387	235
1388	235
1389	235
1390	235
1391	235
1392	235
1393	235
1398	237
1399	237
1400	237
1401	237
1402	238
1403	238
1404	238
1405	238
1406	238
1407	238
1408	238
1413	240
1414	240
1415	240
1416	240
1417	241
1418	241
1419	241
1420	241
1421	241
1422	241
1423	241
1428	243
1429	243
1430	243
1431	243
1432	244
1433	244
1434	244
1435	244
1436	244
1437	244
1438	244
1443	246
1444	246
1445	246
1446	246
1447	247
1448	247
1449	247
1450	247
1451	247
1452	247
1453	247
1458	249
1459	249
1460	249
1461	249
1462	250
1463	250
1464	250
1465	250
1466	250
1467	250
1468	250
1473	252
1474	252
1475	252
1476	252
1477	253
1478	253
1479	253
1480	253
1481	253
1482	253
1483	253
1488	255
1489	255
1490	255
1491	255
1492	256
1493	256
1494	256
1495	256
1496	256
1497	256
1498	256
1503	258
1504	258
1505	258
1506	258
1507	259
1508	259
1509	259
1510	259
1511	259
1512	259
1513	259
1518	261
1519	261
1520	261
1521	261
1522	262
1523	262
1524	262
1525	262
1526	262
1527	262
1528	262
1533	264
1534	264
1535	264
1536	264
1537	265
1538	265
1539	265
1540	265
1541	265
1542	265
1543	265
1548	267
1549	267
1550	267
1551	267
1552	268
1553	268
1554	268
1555	268
1556	268
1557	268
1558	268
1563	270
1564	270
1565	270
1566	270
1567	271
1568	271
1569	271
1570	271
1571	271
1572	271
1573	271
1578	273
1579	273
1580	273
1581	273
1582	274
1583	274
1584	274
1585	274
1586	274
1587	274
1588	274
1593	276
1594	276
1595	276
1596	276
1597	277
1598	277
1599	277
1600	277
1601	277
1602	277
1603	277
1608	279
1609	279
1610	279
1611	279
1612	280
1613	280
1614	280
1615	280
1616	280
1617	280
1618	280
1623	282
1624	282
1625	282
1626	282
1627	283
1628	283
1629	283
1630	283
1631	283
1632	283
1633	283
1638	285
1639	285
1640	285
1641	285
1642	286
1643	286
1644	286
1645	286
1646	286
1647	286
1648	286
1653	288
1654	288
1655	288
1656	288
1657	289
1658	289
1659	289
1660	289
1661	289
1662	289
1663	289
1668	291
1669	291
1670	291
1671	291
1672	292
1673	292
1674	292
1675	292
1676	292
1677	292
1678	292
1683	294
1684	294
1685	294
1686	294
1687	295
1688	295
1689	295
1690	295
1691	295
1692	295
1693	295
1698	297
1699	297
1700	297
1701	297
1702	298
1703	298
1704	298
1705	298
1706	298
1707	298
1708	298
1713	300
1714	300
1715	300
1716	300
1717	301
1718	301
1719	301
1720	301
1721	301
1722	301
1723	301
1728	303
1729	303
1730	303
1731	303
1732	304
1733	304
1734	304
1735	304
1736	304
1737	304
1738	304
1743	306
1744	306
1745	306
1746	306
1747	307
1748	307
1749	307
1750	307
1751	307
1752	307
1753	307
1758	309
1759	309
1760	309
1761	309
1762	310
1763	310
1764	310
1765	310
1766	310
1767	310
1768	310
1773	312
1774	312
1775	312
1776	312
1777	313
1778	313
1779	313
1780	313
1781	313
1782	313
1783	313
1788	315
1789	315
1790	315
1791	315
1792	316
1793	316
1794	316
1795	316
1796	316
1797	316
1798	316
1803	318
1804	318
1805	318
1806	318
1807	319
1808	319
1809	319
1810	319
1811	319
1812	319
1813	319
1818	321
1819	321
1820	321
1821	321
1822	322
1823	322
1824	322
1825	322
1826	322
1827	322
1828	322
1833	324
1834	324
1835	324
1836	324
1837	325
1838	325
1839	325
1840	325
1841	325
1842	325
1843	325
1848	327
1849	327
1850	327
1851	327
1852	328
1853	328
1854	328
1855	328
1856	328
1857	328
1858	328
1863	330
1864	330
1865	330
1866	330
1867	331
1868	331
1869	331
1870	331
1871	331
1872	331
1873	331
1878	333
1879	333
1880	333
1881	333
1882	334
1883	334
1884	334
1885	334
1886	334
1887	334
1888	334
1893	336
1894	336
1895	336
1896	336
1897	337
1898	337
1899	337
1900	337
1901	337
1902	337
1903	337
1908	339
1909	339
1910	339
1911	339
1912	340
1913	340
1914	340
1915	340
1916	340
1917	340
1918	340
1923	342
1924	342
1925	342
1926	342
1927	343
1928	343
1929	343
1930	343
1931	343
1932	343
1933	343
1938	345
1939	345
1940	345
1941	345
1942	346
1943	346
1944	346
1945	346
1946	346
1947	346
1948	346
1953	348
1954	348
1955	348
1956	348
1957	349
1958	349
1959	349
1960	349
1961	349
1962	349
1963	349
1968	351
1969	351
1970	351
1971	351
1972	352
1973	352
1974	352
1975	352
1976	352
1977	352
1978	352
1983	354
1984	354
1985	354
1986	354
1987	355
1988	355
1989	355
1990	355
1991	355
1992	355
1993	355
1998	357
1999	357
2000	357
2001	357
2002	358
2003	358
2004	358
2005	358
2006	358
2007	358
2008	358
2013	360
2014	360
2015	360
2016	360
2017	361
2018	361
2019	361
2020	361
2021	361
2022	361
2023	361
2028	363
2029	363
2030	363
2031	363
2032	364
2033	364
2034	364
2035	364
2036	364
2037	364
2038	364
2043	366
2044	366
2045	366
2046	366
2047	367
2048	367
2049	367
2050	367
2051	367
2052	367
2053	367
2058	369
2059	369
2060	369
2061	369
2062	370
2063	370
2064	370
2065	370
2066	370
2067	370
2068	370
2073	372
2074	372
2075	372
2076	372
2077	373
2078	373
2079	373
2080	373
2081	373
2082	373
2083	373
2088	375
2089	375
2090	375
2091	375
2092	376
2093	376
2094	376
2095	376
2096	376
2097	376
2098	376
2103	378
2104	378
2105	378
2106	378
2107	379
2108	379
2109	379
2110	379
2111	379
2112	379
2113	379
2118	381
2119	381
2120	381
2121	381
2122	382
2123	382
2124	382
2125	382
2126	382
2127	382
2128	382
2133	384
2134	384
2135	384
2136	384
2137	385
2138	385
2139	385
2140	385
2141	385
2142	385
2143	385
2148	387
2149	387
2150	387
2151	387
2152	388
2153	388
2154	388
2155	388
2156	388
2157	388
2158	388
2163	390
2164	390
2165	390
2166	390
2167	391
2168	391
2169	391
2170	391
2171	391
2172	391
2173	391
2178	393
2179	393
2180	393
2181	393
2182	394
2183	394
2184	394
2185	394
2186	394
2187	394
2188	394
2193	396
2194	396
2195	396
2196	396
2197	397
2198	397
2199	397
2200	397
2201	397
2202	397
2203	397
2208	399
2209	399
2210	399
2211	399
2212	400
2213	400
2214	400
2215	400
2216	400
2217	400
2218	400
2223	402
2224	402
2225	402
2226	402
2227	403
2228	403
2229	403
2230	403
2231	403
2232	403
2233	403
2238	405
2239	405
2240	405
2241	405
2242	406
2243	406
2244	406
2245	406
2246	406
2247	406
2248	406
2253	408
2254	408
2255	408
2256	408
2257	409
2258	409
2259	409
2260	409
2261	409
2262	409
2263	409
2268	411
2269	411
2270	411
2271	411
2272	412
2273	412
2274	412
2275	412
2276	412
2277	412
2278	412
2283	414
2284	414
2285	414
2286	414
2287	415
2288	415
2289	415
2290	415
2291	415
2292	415
2293	415
2298	417
2299	417
2300	417
2301	417
2302	418
2303	418
2304	418
2305	418
2306	418
2307	418
2308	418
2313	420
2314	420
2315	420
2316	420
2317	421
2318	421
2319	421
2320	421
2321	421
2322	421
2323	421
2328	423
2329	423
2330	423
2331	423
2332	424
2333	424
2334	424
2335	424
2336	424
2337	424
2338	424
2343	426
2344	426
2345	426
2346	426
2347	427
2348	427
2349	427
2350	427
2351	427
2352	427
2353	427
2358	429
2359	429
2360	429
2361	429
2362	430
2363	430
2364	430
2365	430
2366	430
2367	430
2368	430
2373	432
2374	432
2375	432
2376	432
2377	433
2378	433
2379	433
2380	433
2381	433
2382	433
2383	433
2388	435
2389	435
2390	435
2391	435
2392	436
2393	436
2394	436
2395	436
2396	436
2397	436
2398	436
2403	438
2404	438
2405	438
2406	438
\.


--
-- TOC entry 4488 (class 0 OID 22381)
-- Dependencies: 238
-- Data for Name: IntegerElement; Type: TABLE DATA; Schema: dbo; Owner: -
--

COPY dbo."IntegerElement" ("IntegerElementId", "Value") FROM stdin;
1064	9234
2261	9234
1083	9234
114	9234
2280	9234
1121	9234
152	9234
1140	9234
190	9234
2318	9234
228	9234
1178	9234
266	9234
1197	9234
2337	9234
304	9234
1235	9234
342	9234
1254	9234
380	9234
2375	9234
399	9234
1292	9234
437	9234
2394	9234
456	9234
1311	9234
494	9234
513	9234
1349	9234
2432	9234
551	9234
1368	9234
570	9234
2451	9234
608	9234
1406	9234
627	9234
1425	9234
665	9234
684	9234
2489	9234
1463	9234
722	9234
741	9234
1482	9234
2508	9234
779	9234
798	9234
1520	9234
836	9234
1539	9234
855	9234
2546	9234
893	9234
1577	9234
912	9234
2565	9234
1596	9234
950	9234
969	9234
1634	9234
1007	9234
2603	9234
1026	9234
1653	9234
2622	9234
1691	9234
1710	9234
2660	9234
1748	9234
1767	9234
2679	9234
1805	9234
1824	9234
2717	9234
1862	9234
2736	9234
1881	9234
1919	9234
2774	9234
1938	9234
2793	9234
1976	9234
1995	9234
2831	9234
2033	9234
2052	9234
2850	9234
2090	9234
2109	9234
2888	9234
2147	9234
2907	9234
2166	9234
2204	9234
2945	9234
2223	9234
2964	9234
3002	9234
3021	9234
\.


--
-- TOC entry 4489 (class 0 OID 22394)
-- Dependencies: 239
-- Data for Name: MoneyElement; Type: TABLE DATA; Schema: dbo; Owner: -
--

COPY dbo."MoneyElement" ("MoneyElementId", "Value") FROM stdin;
41	$75,100.35
1400	$75,100.35
60	$75,100.35
1409	$75,100.35
79	$75,100.35
89	$75,100.35
1419	$75,100.35
108	$75,100.35
117	$75,100.35
1428	$75,100.35
127	$75,100.35
146	$75,100.35
155	$75,100.35
1438	$75,100.35
165	$75,100.35
184	$75,100.35
193	$75,100.35
1457	$75,100.35
203	$75,100.35
1466	$75,100.35
222	$75,100.35
231	$75,100.35
241	$75,100.35
1476	$75,100.35
260	$75,100.35
269	$75,100.35
1485	$75,100.35
279	$75,100.35
298	$75,100.35
307	$75,100.35
1495	$75,100.35
317	$75,100.35
336	$75,100.35
345	$75,100.35
1514	$75,100.35
355	$75,100.35
1523	$75,100.35
374	$75,100.35
383	$75,100.35
393	$75,100.35
402	$75,100.35
1533	$75,100.35
412	$75,100.35
1542	$75,100.35
431	$75,100.35
440	$75,100.35
450	$75,100.35
459	$75,100.35
1552	$75,100.35
469	$75,100.35
488	$75,100.35
497	$75,100.35
1571	$75,100.35
507	$75,100.35
516	$75,100.35
1580	$75,100.35
526	$75,100.35
545	$75,100.35
554	$75,100.35
1590	$75,100.35
564	$75,100.35
573	$75,100.35
1599	$75,100.35
583	$75,100.35
602	$75,100.35
611	$75,100.35
1609	$75,100.35
621	$75,100.35
630	$75,100.35
640	$75,100.35
1628	$75,100.35
659	$75,100.35
668	$75,100.35
1637	$75,100.35
678	$75,100.35
687	$75,100.35
697	$75,100.35
1647	$75,100.35
716	$75,100.35
725	$75,100.35
1656	$75,100.35
735	$75,100.35
744	$75,100.35
754	$75,100.35
1666	$75,100.35
773	$75,100.35
782	$75,100.35
792	$75,100.35
801	$75,100.35
1685	$75,100.35
811	$75,100.35
1694	$75,100.35
830	$75,100.35
839	$75,100.35
849	$75,100.35
858	$75,100.35
1704	$75,100.35
868	$75,100.35
1713	$75,100.35
887	$75,100.35
896	$75,100.35
906	$75,100.35
915	$75,100.35
1723	$75,100.35
925	$75,100.35
944	$75,100.35
953	$75,100.35
1742	$75,100.35
963	$75,100.35
972	$75,100.35
1751	$75,100.35
982	$75,100.35
1001	$75,100.35
1010	$75,100.35
1761	$75,100.35
1020	$75,100.35
1029	$75,100.35
1770	$75,100.35
1039	$75,100.35
1058	$75,100.35
1067	$75,100.35
1780	$75,100.35
1077	$75,100.35
1086	$75,100.35
1096	$75,100.35
1799	$75,100.35
1115	$75,100.35
1124	$75,100.35
1808	$75,100.35
1134	$75,100.35
1143	$75,100.35
1153	$75,100.35
1818	$75,100.35
1172	$75,100.35
1181	$75,100.35
1827	$75,100.35
1191	$75,100.35
1200	$75,100.35
1210	$75,100.35
1837	$75,100.35
1229	$75,100.35
1238	$75,100.35
1248	$75,100.35
1257	$75,100.35
1856	$75,100.35
1267	$75,100.35
1865	$75,100.35
1286	$75,100.35
1295	$75,100.35
1305	$75,100.35
1314	$75,100.35
1875	$75,100.35
1324	$75,100.35
1884	$75,100.35
1343	$75,100.35
1352	$75,100.35
1362	$75,100.35
1371	$75,100.35
1894	$75,100.35
1381	$75,100.35
1913	$75,100.35
1922	$75,100.35
1932	$75,100.35
1941	$75,100.35
1951	$75,100.35
1970	$75,100.35
1979	$75,100.35
1989	$75,100.35
1998	$75,100.35
2008	$75,100.35
2027	$75,100.35
2036	$75,100.35
2046	$75,100.35
2055	$75,100.35
2065	$75,100.35
2084	$75,100.35
2093	$75,100.35
2103	$75,100.35
2112	$75,100.35
2122	$75,100.35
2141	$75,100.35
2150	$75,100.35
2160	$75,100.35
2169	$75,100.35
2179	$75,100.35
2198	$75,100.35
2207	$75,100.35
2217	$75,100.35
2226	$75,100.35
2236	$75,100.35
2255	$75,100.35
2264	$75,100.35
2274	$75,100.35
2283	$75,100.35
2293	$75,100.35
2312	$75,100.35
2321	$75,100.35
2331	$75,100.35
2340	$75,100.35
2350	$75,100.35
2369	$75,100.35
2378	$75,100.35
2388	$75,100.35
2397	$75,100.35
2407	$75,100.35
2426	$75,100.35
2435	$75,100.35
2445	$75,100.35
2454	$75,100.35
2464	$75,100.35
2483	$75,100.35
2492	$75,100.35
2502	$75,100.35
2511	$75,100.35
2521	$75,100.35
2540	$75,100.35
2549	$75,100.35
2559	$75,100.35
2568	$75,100.35
2578	$75,100.35
2597	$75,100.35
2606	$75,100.35
2616	$75,100.35
2625	$75,100.35
2635	$75,100.35
2654	$75,100.35
2663	$75,100.35
2673	$75,100.35
2682	$75,100.35
2692	$75,100.35
2711	$75,100.35
2720	$75,100.35
2730	$75,100.35
2739	$75,100.35
2749	$75,100.35
2768	$75,100.35
2777	$75,100.35
2787	$75,100.35
2796	$75,100.35
2806	$75,100.35
2825	$75,100.35
2834	$75,100.35
2844	$75,100.35
2853	$75,100.35
2863	$75,100.35
2882	$75,100.35
2891	$75,100.35
2901	$75,100.35
2910	$75,100.35
2920	$75,100.35
2939	$75,100.35
2948	$75,100.35
2958	$75,100.35
2967	$75,100.35
2977	$75,100.35
2996	$75,100.35
3005	$75,100.35
3015	$75,100.35
3024	$75,100.35
3034	$75,100.35
\.


--
-- TOC entry 4466 (class 0 OID 21422)
-- Dependencies: 216
-- Data for Name: OtherAggregate; Type: TABLE DATA; Schema: dbo; Owner: -
--

COPY dbo."OtherAggregate" ("OtherAggregateId", "Name", "AggregateOptionTypeId") FROM stdin;
\.


--
-- TOC entry 4464 (class 0 OID 21271)
-- Dependencies: 214
-- Data for Name: SubContainer; Type: TABLE DATA; Schema: dbo; Owner: -
--

COPY dbo."SubContainer" ("SubContainerId", "TopContainerId", "Name") FROM stdin;
\.


--
-- TOC entry 4454 (class 0 OID 21111)
-- Dependencies: 204
-- Data for Name: Template; Type: TABLE DATA; Schema: dbo; Owner: -
--

COPY dbo."Template" ("TemplateId", "Name") FROM stdin;
\.


--
-- TOC entry 4490 (class 0 OID 22404)
-- Dependencies: 240
-- Data for Name: TextElement; Type: TABLE DATA; Schema: dbo; Owner: -
--

COPY dbo."TextElement" ("TextElementId", "Value") FROM stdin;
44	423-222-2252
45	615-982-0012
46	+1-555-252-5521
63	423-222-2252
64	615-982-0012
65	+1-555-252-5521
82	423-222-2252
83	615-982-0012
84	+1-555-252-5521
92	423-222-2252
93	615-982-0012
94	+1-555-252-5521
111	423-222-2252
112	615-982-0012
113	+1-555-252-5521
115	Dan
116	The Man
120	423-222-2252
121	615-982-0012
122	+1-555-252-5521
130	423-222-2252
131	615-982-0012
132	+1-555-252-5521
149	423-222-2252
150	615-982-0012
151	+1-555-252-5521
153	Dan
154	The Man
158	423-222-2252
159	615-982-0012
160	+1-555-252-5521
168	423-222-2252
169	615-982-0012
170	+1-555-252-5521
187	423-222-2252
188	615-982-0012
189	+1-555-252-5521
191	Dan
192	The Man
196	423-222-2252
197	615-982-0012
198	+1-555-252-5521
206	423-222-2252
207	615-982-0012
208	+1-555-252-5521
225	423-222-2252
226	615-982-0012
227	+1-555-252-5521
229	Dan
230	The Man
234	423-222-2252
235	615-982-0012
236	+1-555-252-5521
244	423-222-2252
245	615-982-0012
246	+1-555-252-5521
263	423-222-2252
264	615-982-0012
265	+1-555-252-5521
267	Dan
268	The Man
272	423-222-2252
273	615-982-0012
274	+1-555-252-5521
282	423-222-2252
283	615-982-0012
284	+1-555-252-5521
301	423-222-2252
302	615-982-0012
303	+1-555-252-5521
305	Dan
306	The Man
310	423-222-2252
311	615-982-0012
312	+1-555-252-5521
320	423-222-2252
321	615-982-0012
322	+1-555-252-5521
339	423-222-2252
340	615-982-0012
341	+1-555-252-5521
343	Dan
344	The Man
348	423-222-2252
349	615-982-0012
350	+1-555-252-5521
358	423-222-2252
359	615-982-0012
360	+1-555-252-5521
377	423-222-2252
378	615-982-0012
379	+1-555-252-5521
381	Dan
382	The Man
386	423-222-2252
387	615-982-0012
388	+1-555-252-5521
396	423-222-2252
397	615-982-0012
398	+1-555-252-5521
400	Dan
401	The Man
405	423-222-2252
406	615-982-0012
407	+1-555-252-5521
415	423-222-2252
416	615-982-0012
417	+1-555-252-5521
434	423-222-2252
435	615-982-0012
436	+1-555-252-5521
438	Dan
439	The Man
443	423-222-2252
444	615-982-0012
445	+1-555-252-5521
453	423-222-2252
454	615-982-0012
455	+1-555-252-5521
457	Dan
458	The Man
462	423-222-2252
463	615-982-0012
464	+1-555-252-5521
472	423-222-2252
473	615-982-0012
474	+1-555-252-5521
491	423-222-2252
492	615-982-0012
493	+1-555-252-5521
495	Dan
496	The Man
500	423-222-2252
501	615-982-0012
502	+1-555-252-5521
510	423-222-2252
511	615-982-0012
512	+1-555-252-5521
514	Dan
515	The Man
519	423-222-2252
520	615-982-0012
521	+1-555-252-5521
529	423-222-2252
530	615-982-0012
531	+1-555-252-5521
548	423-222-2252
549	615-982-0012
550	+1-555-252-5521
552	Dan
553	The Man
557	423-222-2252
558	615-982-0012
559	+1-555-252-5521
567	423-222-2252
568	615-982-0012
569	+1-555-252-5521
571	Dan
572	The Man
576	423-222-2252
577	615-982-0012
578	+1-555-252-5521
586	423-222-2252
587	615-982-0012
588	+1-555-252-5521
605	423-222-2252
606	615-982-0012
607	+1-555-252-5521
609	Dan
610	The Man
614	423-222-2252
615	615-982-0012
616	+1-555-252-5521
624	423-222-2252
625	615-982-0012
626	+1-555-252-5521
628	Dan
629	The Man
633	423-222-2252
634	615-982-0012
635	+1-555-252-5521
643	423-222-2252
644	615-982-0012
645	+1-555-252-5521
662	423-222-2252
663	615-982-0012
664	+1-555-252-5521
666	Dan
667	The Man
671	423-222-2252
672	615-982-0012
673	+1-555-252-5521
681	423-222-2252
682	615-982-0012
683	+1-555-252-5521
685	Dan
686	The Man
690	423-222-2252
691	615-982-0012
692	+1-555-252-5521
700	423-222-2252
701	615-982-0012
702	+1-555-252-5521
719	423-222-2252
720	615-982-0012
721	+1-555-252-5521
723	Dan
724	The Man
728	423-222-2252
729	615-982-0012
730	+1-555-252-5521
738	423-222-2252
739	615-982-0012
740	+1-555-252-5521
742	Dan
743	The Man
747	423-222-2252
748	615-982-0012
749	+1-555-252-5521
757	423-222-2252
758	615-982-0012
759	+1-555-252-5521
776	423-222-2252
777	615-982-0012
778	+1-555-252-5521
780	Dan
781	The Man
785	423-222-2252
786	615-982-0012
787	+1-555-252-5521
795	423-222-2252
796	615-982-0012
797	+1-555-252-5521
799	Dan
800	The Man
804	423-222-2252
805	615-982-0012
806	+1-555-252-5521
814	423-222-2252
815	615-982-0012
816	+1-555-252-5521
833	423-222-2252
834	615-982-0012
835	+1-555-252-5521
837	Dan
838	The Man
842	423-222-2252
843	615-982-0012
844	+1-555-252-5521
852	423-222-2252
853	615-982-0012
854	+1-555-252-5521
856	Dan
857	The Man
861	423-222-2252
862	615-982-0012
863	+1-555-252-5521
871	423-222-2252
872	615-982-0012
873	+1-555-252-5521
890	423-222-2252
891	615-982-0012
892	+1-555-252-5521
894	Dan
895	The Man
899	423-222-2252
900	615-982-0012
901	+1-555-252-5521
909	423-222-2252
910	615-982-0012
911	+1-555-252-5521
913	Dan
914	The Man
918	423-222-2252
919	615-982-0012
920	+1-555-252-5521
928	423-222-2252
929	615-982-0012
930	+1-555-252-5521
947	423-222-2252
948	615-982-0012
949	+1-555-252-5521
951	Dan
952	The Man
956	423-222-2252
957	615-982-0012
958	+1-555-252-5521
966	423-222-2252
967	615-982-0012
968	+1-555-252-5521
970	Dan
971	The Man
975	423-222-2252
976	615-982-0012
977	+1-555-252-5521
985	423-222-2252
986	615-982-0012
987	+1-555-252-5521
1004	423-222-2252
1005	615-982-0012
1006	+1-555-252-5521
1008	Dan
1009	The Man
1013	423-222-2252
1014	615-982-0012
1015	+1-555-252-5521
1023	423-222-2252
1024	615-982-0012
1025	+1-555-252-5521
1027	Dan
1028	The Man
1032	423-222-2252
1033	615-982-0012
1034	+1-555-252-5521
1061	423-222-2252
1062	615-982-0012
1042	423-222-2252
1043	615-982-0012
1044	+1-555-252-5521
1063	+1-555-252-5521
1065	Dan
1066	The Man
1070	423-222-2252
1071	615-982-0012
1072	+1-555-252-5521
1080	423-222-2252
1081	615-982-0012
1082	+1-555-252-5521
1084	Dan
1085	The Man
1089	423-222-2252
1090	615-982-0012
1091	+1-555-252-5521
1099	423-222-2252
1100	615-982-0012
1101	+1-555-252-5521
1118	423-222-2252
1119	615-982-0012
1120	+1-555-252-5521
1122	Dan
1123	The Man
1127	423-222-2252
1128	615-982-0012
1129	+1-555-252-5521
1137	423-222-2252
1138	615-982-0012
1139	+1-555-252-5521
1141	Dan
1142	The Man
1146	423-222-2252
1147	615-982-0012
1148	+1-555-252-5521
1156	423-222-2252
1157	615-982-0012
1158	+1-555-252-5521
1175	423-222-2252
1176	615-982-0012
1177	+1-555-252-5521
1179	Dan
1180	The Man
1184	423-222-2252
1185	615-982-0012
1186	+1-555-252-5521
1194	423-222-2252
1195	615-982-0012
1196	+1-555-252-5521
1198	Dan
1199	The Man
1203	423-222-2252
1204	615-982-0012
1205	+1-555-252-5521
1213	423-222-2252
1214	615-982-0012
1215	+1-555-252-5521
1232	423-222-2252
1233	615-982-0012
1234	+1-555-252-5521
1236	Dan
1237	The Man
1241	423-222-2252
1242	615-982-0012
1243	+1-555-252-5521
1251	423-222-2252
1252	615-982-0012
1253	+1-555-252-5521
1255	Dan
1256	The Man
1260	423-222-2252
1261	615-982-0012
1262	+1-555-252-5521
1270	423-222-2252
1271	615-982-0012
1272	+1-555-252-5521
1289	423-222-2252
1290	615-982-0012
1291	+1-555-252-5521
1293	Dan
1294	The Man
1298	423-222-2252
1299	615-982-0012
1300	+1-555-252-5521
1308	423-222-2252
1309	615-982-0012
1310	+1-555-252-5521
1312	Dan
1313	The Man
1317	423-222-2252
1318	615-982-0012
1319	+1-555-252-5521
1327	423-222-2252
1328	615-982-0012
1329	+1-555-252-5521
1346	423-222-2252
1347	615-982-0012
1348	+1-555-252-5521
1350	Dan
1351	The Man
1355	423-222-2252
1356	615-982-0012
1357	+1-555-252-5521
1365	423-222-2252
1366	615-982-0012
1367	+1-555-252-5521
1369	Dan
1370	The Man
1374	423-222-2252
1375	615-982-0012
1376	+1-555-252-5521
1384	423-222-2252
1385	615-982-0012
1386	+1-555-252-5521
1403	423-222-2252
1404	615-982-0012
1405	+1-555-252-5521
1407	Dan
1408	The Man
1412	423-222-2252
1413	615-982-0012
1414	+1-555-252-5521
1422	423-222-2252
1423	615-982-0012
1424	+1-555-252-5521
1426	Dan
1427	The Man
1431	423-222-2252
1432	615-982-0012
1433	+1-555-252-5521
1441	423-222-2252
1442	615-982-0012
1443	+1-555-252-5521
1460	423-222-2252
1461	615-982-0012
1462	+1-555-252-5521
1464	Dan
1465	The Man
1469	423-222-2252
1470	615-982-0012
1471	+1-555-252-5521
1479	423-222-2252
1480	615-982-0012
1481	+1-555-252-5521
1483	Dan
1484	The Man
1488	423-222-2252
1489	615-982-0012
1490	+1-555-252-5521
1498	423-222-2252
1499	615-982-0012
1500	+1-555-252-5521
1517	423-222-2252
1518	615-982-0012
1519	+1-555-252-5521
1521	Dan
1522	The Man
1526	423-222-2252
1527	615-982-0012
1528	+1-555-252-5521
1536	423-222-2252
1537	615-982-0012
1538	+1-555-252-5521
1540	Dan
1541	The Man
1545	423-222-2252
1546	615-982-0012
1547	+1-555-252-5521
1555	423-222-2252
1556	615-982-0012
1557	+1-555-252-5521
1574	423-222-2252
1575	615-982-0012
1576	+1-555-252-5521
1578	Dan
1579	The Man
1583	423-222-2252
1584	615-982-0012
1585	+1-555-252-5521
1593	423-222-2252
1594	615-982-0012
1595	+1-555-252-5521
1597	Dan
1598	The Man
1602	423-222-2252
1603	615-982-0012
1604	+1-555-252-5521
1612	423-222-2252
1613	615-982-0012
1614	+1-555-252-5521
1631	423-222-2252
1632	615-982-0012
1633	+1-555-252-5521
1635	Dan
1636	The Man
1640	423-222-2252
1641	615-982-0012
1642	+1-555-252-5521
1650	423-222-2252
1651	615-982-0012
1652	+1-555-252-5521
1654	Dan
1655	The Man
1659	423-222-2252
1660	615-982-0012
1661	+1-555-252-5521
1669	423-222-2252
1670	615-982-0012
1671	+1-555-252-5521
1688	423-222-2252
1689	615-982-0012
1690	+1-555-252-5521
1692	Dan
1693	The Man
1697	423-222-2252
1698	615-982-0012
1699	+1-555-252-5521
1707	423-222-2252
1708	615-982-0012
1709	+1-555-252-5521
1711	Dan
1712	The Man
1716	423-222-2252
1717	615-982-0012
1718	+1-555-252-5521
1726	423-222-2252
1727	615-982-0012
1728	+1-555-252-5521
1745	423-222-2252
1746	615-982-0012
1747	+1-555-252-5521
1749	Dan
1750	The Man
1754	423-222-2252
1755	615-982-0012
1756	+1-555-252-5521
1764	423-222-2252
1765	615-982-0012
1766	+1-555-252-5521
1768	Dan
1769	The Man
1773	423-222-2252
1774	615-982-0012
1775	+1-555-252-5521
1783	423-222-2252
1784	615-982-0012
1785	+1-555-252-5521
1802	423-222-2252
1803	615-982-0012
1804	+1-555-252-5521
1806	Dan
1807	The Man
1811	423-222-2252
1812	615-982-0012
1813	+1-555-252-5521
1821	423-222-2252
1822	615-982-0012
1823	+1-555-252-5521
1825	Dan
1826	The Man
1830	423-222-2252
1831	615-982-0012
1832	+1-555-252-5521
1840	423-222-2252
1841	615-982-0012
1842	+1-555-252-5521
1859	423-222-2252
1860	615-982-0012
1861	+1-555-252-5521
1863	Dan
1864	The Man
1868	423-222-2252
1869	615-982-0012
1870	+1-555-252-5521
1878	423-222-2252
1879	615-982-0012
1880	+1-555-252-5521
1882	Dan
1883	The Man
1887	423-222-2252
1888	615-982-0012
1889	+1-555-252-5521
1897	423-222-2252
1898	615-982-0012
1899	+1-555-252-5521
1916	423-222-2252
1917	615-982-0012
1918	+1-555-252-5521
1920	Dan
1921	The Man
1925	423-222-2252
1926	615-982-0012
1927	+1-555-252-5521
1935	423-222-2252
1936	615-982-0012
1937	+1-555-252-5521
1939	Dan
1940	The Man
1944	423-222-2252
1945	615-982-0012
1946	+1-555-252-5521
1954	423-222-2252
1955	615-982-0012
1956	+1-555-252-5521
1973	423-222-2252
1974	615-982-0012
1975	+1-555-252-5521
1977	Dan
1978	The Man
1982	423-222-2252
1983	615-982-0012
1984	+1-555-252-5521
1992	423-222-2252
1993	615-982-0012
1994	+1-555-252-5521
1996	Dan
1997	The Man
2001	423-222-2252
2002	615-982-0012
2003	+1-555-252-5521
2011	423-222-2252
2012	615-982-0012
2013	+1-555-252-5521
2030	423-222-2252
2031	615-982-0012
2032	+1-555-252-5521
2034	Dan
2035	The Man
2039	423-222-2252
2040	615-982-0012
2041	+1-555-252-5521
2049	423-222-2252
2050	615-982-0012
2051	+1-555-252-5521
2053	Dan
2054	The Man
2058	423-222-2252
2059	615-982-0012
2060	+1-555-252-5521
2068	423-222-2252
2069	615-982-0012
2070	+1-555-252-5521
2087	423-222-2252
2088	615-982-0012
2089	+1-555-252-5521
2091	Dan
2092	The Man
2096	423-222-2252
2097	615-982-0012
2098	+1-555-252-5521
2106	423-222-2252
2107	615-982-0012
2108	+1-555-252-5521
2110	Dan
2111	The Man
2115	423-222-2252
2116	615-982-0012
2117	+1-555-252-5521
2125	423-222-2252
2126	615-982-0012
2127	+1-555-252-5521
2144	423-222-2252
2145	615-982-0012
2146	+1-555-252-5521
2148	Dan
2149	The Man
2153	423-222-2252
2154	615-982-0012
2155	+1-555-252-5521
2163	423-222-2252
2164	615-982-0012
2165	+1-555-252-5521
2167	Dan
2168	The Man
2172	423-222-2252
2173	615-982-0012
2174	+1-555-252-5521
2182	423-222-2252
2183	615-982-0012
2184	+1-555-252-5521
2201	423-222-2252
2202	615-982-0012
2203	+1-555-252-5521
2205	Dan
2206	The Man
2210	423-222-2252
2211	615-982-0012
2212	+1-555-252-5521
2220	423-222-2252
2221	615-982-0012
2222	+1-555-252-5521
2224	Dan
2225	The Man
2229	423-222-2252
2230	615-982-0012
2231	+1-555-252-5521
2239	423-222-2252
2240	615-982-0012
2241	+1-555-252-5521
2258	423-222-2252
2259	615-982-0012
2260	+1-555-252-5521
2262	Dan
2263	The Man
2267	423-222-2252
2268	615-982-0012
2269	+1-555-252-5521
2277	423-222-2252
2278	615-982-0012
2279	+1-555-252-5521
2281	Dan
2282	The Man
2286	423-222-2252
2287	615-982-0012
2288	+1-555-252-5521
2296	423-222-2252
2297	615-982-0012
2298	+1-555-252-5521
2315	423-222-2252
2316	615-982-0012
2317	+1-555-252-5521
2319	Dan
2320	The Man
2324	423-222-2252
2325	615-982-0012
2326	+1-555-252-5521
2334	423-222-2252
2335	615-982-0012
2336	+1-555-252-5521
2338	Dan
2339	The Man
2343	423-222-2252
2344	615-982-0012
2345	+1-555-252-5521
2353	423-222-2252
2354	615-982-0012
2355	+1-555-252-5521
2372	423-222-2252
2373	615-982-0012
2374	+1-555-252-5521
2376	Dan
2377	The Man
2381	423-222-2252
2382	615-982-0012
2383	+1-555-252-5521
2391	423-222-2252
2392	615-982-0012
2393	+1-555-252-5521
2395	Dan
2396	The Man
2400	423-222-2252
2401	615-982-0012
2402	+1-555-252-5521
2410	423-222-2252
2411	615-982-0012
2412	+1-555-252-5521
2429	423-222-2252
2430	615-982-0012
2431	+1-555-252-5521
2433	Dan
2434	The Man
2438	423-222-2252
2439	615-982-0012
2440	+1-555-252-5521
2448	423-222-2252
2449	615-982-0012
2450	+1-555-252-5521
2452	Dan
2453	The Man
2457	423-222-2252
2458	615-982-0012
2459	+1-555-252-5521
2467	423-222-2252
2468	615-982-0012
2469	+1-555-252-5521
2486	423-222-2252
2487	615-982-0012
2488	+1-555-252-5521
2490	Dan
2491	The Man
2495	423-222-2252
2496	615-982-0012
2497	+1-555-252-5521
2505	423-222-2252
2506	615-982-0012
2507	+1-555-252-5521
2509	Dan
2510	The Man
2514	423-222-2252
2515	615-982-0012
2516	+1-555-252-5521
2524	423-222-2252
2525	615-982-0012
2526	+1-555-252-5521
2543	423-222-2252
2544	615-982-0012
2545	+1-555-252-5521
2547	Dan
2548	The Man
2552	423-222-2252
2553	615-982-0012
2554	+1-555-252-5521
2562	423-222-2252
2563	615-982-0012
2564	+1-555-252-5521
2566	Dan
2567	The Man
2571	423-222-2252
2572	615-982-0012
2573	+1-555-252-5521
2581	423-222-2252
2582	615-982-0012
2583	+1-555-252-5521
2600	423-222-2252
2601	615-982-0012
2602	+1-555-252-5521
2604	Dan
2605	The Man
2609	423-222-2252
2610	615-982-0012
2611	+1-555-252-5521
2619	423-222-2252
2620	615-982-0012
2621	+1-555-252-5521
2623	Dan
2624	The Man
2628	423-222-2252
2629	615-982-0012
2630	+1-555-252-5521
2638	423-222-2252
2639	615-982-0012
2640	+1-555-252-5521
2657	423-222-2252
2658	615-982-0012
2659	+1-555-252-5521
2661	Dan
2662	The Man
2666	423-222-2252
2667	615-982-0012
2668	+1-555-252-5521
2676	423-222-2252
2677	615-982-0012
2678	+1-555-252-5521
2680	Dan
2681	The Man
2685	423-222-2252
2686	615-982-0012
2687	+1-555-252-5521
2695	423-222-2252
2696	615-982-0012
2697	+1-555-252-5521
2714	423-222-2252
2715	615-982-0012
2716	+1-555-252-5521
2718	Dan
2719	The Man
2723	423-222-2252
2724	615-982-0012
2725	+1-555-252-5521
2733	423-222-2252
2734	615-982-0012
2735	+1-555-252-5521
2737	Dan
2738	The Man
2742	423-222-2252
2743	615-982-0012
2744	+1-555-252-5521
2752	423-222-2252
2753	615-982-0012
2754	+1-555-252-5521
2771	423-222-2252
2772	615-982-0012
2773	+1-555-252-5521
2775	Dan
2776	The Man
2780	423-222-2252
2781	615-982-0012
2782	+1-555-252-5521
2790	423-222-2252
2791	615-982-0012
2792	+1-555-252-5521
2794	Dan
2795	The Man
2799	423-222-2252
2800	615-982-0012
2801	+1-555-252-5521
2809	423-222-2252
2810	615-982-0012
2811	+1-555-252-5521
2828	423-222-2252
2829	615-982-0012
2830	+1-555-252-5521
2832	Dan
2833	The Man
2837	423-222-2252
2838	615-982-0012
2839	+1-555-252-5521
2847	423-222-2252
2848	615-982-0012
2849	+1-555-252-5521
2851	Dan
2852	The Man
2856	423-222-2252
2857	615-982-0012
2858	+1-555-252-5521
2866	423-222-2252
2867	615-982-0012
2868	+1-555-252-5521
2885	423-222-2252
2886	615-982-0012
2887	+1-555-252-5521
2889	Dan
2890	The Man
2894	423-222-2252
2895	615-982-0012
2896	+1-555-252-5521
2904	423-222-2252
2905	615-982-0012
2906	+1-555-252-5521
2908	Dan
2909	The Man
2913	423-222-2252
2914	615-982-0012
2915	+1-555-252-5521
2923	423-222-2252
2924	615-982-0012
2925	+1-555-252-5521
2942	423-222-2252
2943	615-982-0012
2944	+1-555-252-5521
2946	Dan
2947	The Man
2951	423-222-2252
2952	615-982-0012
2953	+1-555-252-5521
2961	423-222-2252
2962	615-982-0012
2963	+1-555-252-5521
2965	Dan
2966	The Man
2970	423-222-2252
2971	615-982-0012
2972	+1-555-252-5521
2980	423-222-2252
2981	615-982-0012
2982	+1-555-252-5521
2999	423-222-2252
3000	615-982-0012
3001	+1-555-252-5521
3003	Dan
3004	The Man
3008	423-222-2252
3009	615-982-0012
3010	+1-555-252-5521
3018	423-222-2252
3019	615-982-0012
3020	+1-555-252-5521
3022	Dan
3023	The Man
3027	423-222-2252
3028	615-982-0012
3029	+1-555-252-5521
3037	423-222-2252
3038	615-982-0012
3039	+1-555-252-5521
\.


--
-- TOC entry 4452 (class 0 OID 21100)
-- Dependencies: 202
-- Data for Name: TopContainer; Type: TABLE DATA; Schema: dbo; Owner: -
--

COPY dbo."TopContainer" ("TopContainerId", "Name") FROM stdin;
\.


--
-- TOC entry 4511 (class 0 OID 0)
-- Dependencies: 223
-- Name: AggregateEventStartId_AggregateEventStartId_seq; Type: SEQUENCE SET; Schema: dbo; Owner: -
--

SELECT pg_catalog.setval('dbo."AggregateEventStartId_AggregateEventStartId_seq"', 1, false);


--
-- TOC entry 4512 (class 0 OID 0)
-- Dependencies: 206
-- Name: CategoryAttribute_CategoryAttributeId_seq; Type: SEQUENCE SET; Schema: dbo; Owner: -
--

SELECT pg_catalog.setval('dbo."CategoryAttribute_CategoryAttributeId_seq"', 635, true);


--
-- TOC entry 4513 (class 0 OID 0)
-- Dependencies: 220
-- Name: Child_ChildId_seq; Type: SEQUENCE SET; Schema: dbo; Owner: -
--

SELECT pg_catalog.setval('dbo."Child_ChildId_seq"', 1, false);


--
-- TOC entry 4514 (class 0 OID 0)
-- Dependencies: 217
-- Name: DomainAggregate_DomainAggregateId_seq; Type: SEQUENCE SET; Schema: dbo; Owner: -
--

SELECT pg_catalog.setval('dbo."DomainAggregate_DomainAggregateId_seq"', 1875, true);


--
-- TOC entry 4515 (class 0 OID 0)
-- Dependencies: 208
-- Name: DomainIdentity_DomainIdentityId_seq; Type: SEQUENCE SET; Schema: dbo; Owner: -
--

SELECT pg_catalog.setval('dbo."DomainIdentity_DomainIdentityId_seq"', 1490, true);


--
-- TOC entry 4516 (class 0 OID 0)
-- Dependencies: 228
-- Name: FieldValueElement_FieldValueElementId_seq; Type: SEQUENCE SET; Schema: dbo; Owner: -
--

SELECT pg_catalog.setval('dbo."FieldValueElement_FieldValueElementId_seq"', 3039, true);


--
-- TOC entry 4517 (class 0 OID 0)
-- Dependencies: 226
-- Name: FieldValue_FieldValueId_seq; Type: SEQUENCE SET; Schema: dbo; Owner: -
--

SELECT pg_catalog.setval('dbo."FieldValue_FieldValueId_seq"', 2406, true);


--
-- TOC entry 4518 (class 0 OID 0)
-- Dependencies: 210
-- Name: Field_FieldId_seq; Type: SEQUENCE SET; Schema: dbo; Owner: -
--

SELECT pg_catalog.setval('dbo."Field_FieldId_seq"', 4411, true);


--
-- TOC entry 4519 (class 0 OID 0)
-- Dependencies: 241
-- Name: FlagAttribute_FlagAttributeId_seq; Type: SEQUENCE SET; Schema: dbo; Owner: -
--

SELECT pg_catalog.setval('dbo."FlagAttribute_FlagAttributeId_seq"', 285, true);


--
-- TOC entry 4520 (class 0 OID 0)
-- Dependencies: 230
-- Name: GenericSubmission_GenericSubmissionId_seq; Type: SEQUENCE SET; Schema: dbo; Owner: -
--

SELECT pg_catalog.setval('dbo."GenericSubmission_GenericSubmissionId_seq"', 438, true);


--
-- TOC entry 4521 (class 0 OID 0)
-- Dependencies: 215
-- Name: OtherAggregate_OtherAggregateId_seq; Type: SEQUENCE SET; Schema: dbo; Owner: -
--

SELECT pg_catalog.setval('dbo."OtherAggregate_OtherAggregateId_seq"', 635, true);


--
-- TOC entry 4522 (class 0 OID 0)
-- Dependencies: 213
-- Name: SubContainer_SubContainerId_seq; Type: SEQUENCE SET; Schema: dbo; Owner: -
--

SELECT pg_catalog.setval('dbo."SubContainer_SubContainerId_seq"', 635, true);


--
-- TOC entry 4523 (class 0 OID 0)
-- Dependencies: 203
-- Name: Template_TemplateId_seq; Type: SEQUENCE SET; Schema: dbo; Owner: -
--

SELECT pg_catalog.setval('dbo."Template_TemplateId_seq"', 625, true);


--
-- TOC entry 4524 (class 0 OID 0)
-- Dependencies: 201
-- Name: TopContainer_TopContainerId_seq; Type: SEQUENCE SET; Schema: dbo; Owner: -
--

SELECT pg_catalog.setval('dbo."TopContainer_TopContainerId_seq"', 635, true);


--
-- TOC entry 4269 (class 2606 OID 21765)
-- Name: AggregateEventCompletion PK_AggregateEventCompletion; Type: CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."AggregateEventCompletion"
    ADD CONSTRAINT "PK_AggregateEventCompletion" PRIMARY KEY ("AggregateEventCompletionId");


--
-- TOC entry 4267 (class 2606 OID 21737)
-- Name: AggregateEventStart PK_AggregateEventStart; Type: CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."AggregateEventStart"
    ADD CONSTRAINT "PK_AggregateEventStart" PRIMARY KEY ("AggregateEventStartId");


--
-- TOC entry 4259 (class 2606 OID 21491)
-- Name: AggregateOption PK_AggregateOption; Type: CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."AggregateOption"
    ADD CONSTRAINT "PK_AggregateOption" PRIMARY KEY ("AggregateOptionId");


--
-- TOC entry 4239 (class 2606 OID 21133)
-- Name: AggregateOptionType PK_AggregateOptionType; Type: CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."AggregateOptionType"
    ADD CONSTRAINT "PK_AggregateOptionType" PRIMARY KEY ("AggregateOptionTypeId");


--
-- TOC entry 4279 (class 2606 OID 22286)
-- Name: AggregateSubmission PK_AggregateSubmission; Type: CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."AggregateSubmission"
    ADD CONSTRAINT "PK_AggregateSubmission" PRIMARY KEY ("AggregateSubmissionId");


--
-- TOC entry 4263 (class 2606 OID 21570)
-- Name: Association PK_Association; Type: CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."Association"
    ADD CONSTRAINT "PK_Association" PRIMARY KEY ("OtherAggregateId");


--
-- TOC entry 4241 (class 2606 OID 21144)
-- Name: CategoryAttribute PK_CategoryAttribute; Type: CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."CategoryAttribute"
    ADD CONSTRAINT "PK_CategoryAttribute" PRIMARY KEY ("CategoryAttributeId");


--
-- TOC entry 4261 (class 2606 OID 21545)
-- Name: Child PK_Child; Type: CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."Child"
    ADD CONSTRAINT "PK_Child" PRIMARY KEY ("ChildId");


--
-- TOC entry 4281 (class 2606 OID 22316)
-- Name: CurrentAggregateSubmission PK_CurrentAggregateSubmission; Type: CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."CurrentAggregateSubmission"
    ADD CONSTRAINT "PK_CurrentAggregateSubmission" PRIMARY KEY ("CurrentAggregateSubmissionId");


--
-- TOC entry 4287 (class 2606 OID 22357)
-- Name: DateElement PK_DateElement; Type: CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."DateElement"
    ADD CONSTRAINT "PK_DateElement" PRIMARY KEY ("DateElementId");


--
-- TOC entry 4257 (class 2606 OID 21458)
-- Name: DomainAggregate PK_DomainAggregate; Type: CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."DomainAggregate"
    ADD CONSTRAINT "PK_DomainAggregate" PRIMARY KEY ("DomainAggregateId");


--
-- TOC entry 4285 (class 2606 OID 22336)
-- Name: DomainAggregateFlagAttribute PK_DomainAggregateFlagAttribute; Type: CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."DomainAggregateFlagAttribute"
    ADD CONSTRAINT "PK_DomainAggregateFlagAttribute" PRIMARY KEY ("DomainAggregateId", "FlagAttributeId");


--
-- TOC entry 4243 (class 2606 OID 21155)
-- Name: DomainIdentity PK_DomainIdentity; Type: CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."DomainIdentity"
    ADD CONSTRAINT "PK_DomainIdentity" PRIMARY KEY ("DomainIdentityId");


--
-- TOC entry 4247 (class 2606 OID 21180)
-- Name: Field PK_Field; Type: CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."Field"
    ADD CONSTRAINT "PK_Field" PRIMARY KEY ("FieldId");


--
-- TOC entry 4271 (class 2606 OID 21781)
-- Name: FieldValue PK_FieldValue; Type: CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."FieldValue"
    ADD CONSTRAINT "PK_FieldValue" PRIMARY KEY ("FieldValueId");


--
-- TOC entry 4273 (class 2606 OID 22210)
-- Name: FieldValueElement PK_FieldValueElement; Type: CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."FieldValueElement"
    ADD CONSTRAINT "PK_FieldValueElement" PRIMARY KEY ("FieldValueElementId");


--
-- TOC entry 4251 (class 2606 OID 21190)
-- Name: FlagAttribute PK_FlagAttribute; Type: CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."FlagAttribute"
    ADD CONSTRAINT "PK_FlagAttribute" PRIMARY KEY ("FlagAttributeId");


--
-- TOC entry 4289 (class 2606 OID 22370)
-- Name: FloatElement PK_FloatElement; Type: CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."FloatElement"
    ADD CONSTRAINT "PK_FloatElement" PRIMARY KEY ("FloatElementId");


--
-- TOC entry 4275 (class 2606 OID 22240)
-- Name: GenericSubmission PK_GenericSubmission; Type: CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."GenericSubmission"
    ADD CONSTRAINT "PK_GenericSubmission" PRIMARY KEY ("GenericSubmissionId");


--
-- TOC entry 4277 (class 2606 OID 22253)
-- Name: GenericSubmissionValue PK_GenericSubmissionValue; Type: CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."GenericSubmissionValue"
    ADD CONSTRAINT "PK_GenericSubmissionValue" PRIMARY KEY ("GenericSubmissionValueId");


--
-- TOC entry 4291 (class 2606 OID 22385)
-- Name: IntegerElement PK_IntegerElement; Type: CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."IntegerElement"
    ADD CONSTRAINT "PK_IntegerElement" PRIMARY KEY ("IntegerElementId");


--
-- TOC entry 4293 (class 2606 OID 22398)
-- Name: MoneyElement PK_MoneyElement; Type: CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."MoneyElement"
    ADD CONSTRAINT "PK_MoneyElement" PRIMARY KEY ("MoneyElementId");


--
-- TOC entry 4255 (class 2606 OID 21427)
-- Name: OtherAggregate PK_OtherAggregate; Type: CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."OtherAggregate"
    ADD CONSTRAINT "PK_OtherAggregate" PRIMARY KEY ("OtherAggregateId");


--
-- TOC entry 4253 (class 2606 OID 21276)
-- Name: SubContainer PK_SubContainerId; Type: CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."SubContainer"
    ADD CONSTRAINT "PK_SubContainerId" PRIMARY KEY ("SubContainerId");


--
-- TOC entry 4237 (class 2606 OID 21116)
-- Name: Template PK_Template; Type: CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."Template"
    ADD CONSTRAINT "PK_Template" PRIMARY KEY ("TemplateId");


--
-- TOC entry 4295 (class 2606 OID 22411)
-- Name: TextElement PK_TextElement; Type: CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."TextElement"
    ADD CONSTRAINT "PK_TextElement" PRIMARY KEY ("TextElementId");


--
-- TOC entry 4235 (class 2606 OID 21105)
-- Name: TopContainer PK_TopContainer; Type: CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."TopContainer"
    ADD CONSTRAINT "PK_TopContainer" PRIMARY KEY ("TopContainerId");


--
-- TOC entry 4265 (class 2606 OID 21572)
-- Name: Association UK_AggregateLink_DomainAggregateId; Type: CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."Association"
    ADD CONSTRAINT "UK_AggregateLink_DomainAggregateId" UNIQUE ("DomainAggregateId");


--
-- TOC entry 4283 (class 2606 OID 22318)
-- Name: CurrentAggregateSubmission UK_CurrentAggregateSubmission_DomainAggregateId; Type: CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."CurrentAggregateSubmission"
    ADD CONSTRAINT "UK_CurrentAggregateSubmission_DomainAggregateId" UNIQUE ("DomainAggregateId");


--
-- TOC entry 4245 (class 2606 OID 21157)
-- Name: DomainIdentity UK_DomainIdentity_UniqueIdentifier; Type: CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."DomainIdentity"
    ADD CONSTRAINT "UK_DomainIdentity_UniqueIdentifier" UNIQUE ("UniqueIdentifier");


--
-- TOC entry 4249 (class 2606 OID 21182)
-- Name: Field UK_Field_Name; Type: CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."Field"
    ADD CONSTRAINT "UK_Field_Name" UNIQUE ("Name");


--
-- TOC entry 4310 (class 2606 OID 21766)
-- Name: AggregateEventCompletion FK_AggregateEventCompletion_AggregateEventStart; Type: FK CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."AggregateEventCompletion"
    ADD CONSTRAINT "FK_AggregateEventCompletion_AggregateEventStart" FOREIGN KEY ("AggregateEventCompletionId") REFERENCES dbo."AggregateEventStart"("AggregateEventStartId") ON DELETE CASCADE;


--
-- TOC entry 4308 (class 2606 OID 21738)
-- Name: AggregateEventStart FK_AggregateEventStart_DomainAggregate; Type: FK CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."AggregateEventStart"
    ADD CONSTRAINT "FK_AggregateEventStart_DomainAggregate" FOREIGN KEY ("DomainAggregateId") REFERENCES dbo."DomainAggregate"("DomainAggregateId");


--
-- TOC entry 4309 (class 2606 OID 21743)
-- Name: AggregateEventStart FK_AggregateEventStart_DomainIdentity; Type: FK CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."AggregateEventStart"
    ADD CONSTRAINT "FK_AggregateEventStart_DomainIdentity" FOREIGN KEY ("DomainIdentityId") REFERENCES dbo."DomainIdentity"("DomainIdentityId");


--
-- TOC entry 4303 (class 2606 OID 21492)
-- Name: AggregateOption FK_AggregateOption_AggregateOptionType; Type: FK CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."AggregateOption"
    ADD CONSTRAINT "FK_AggregateOption_AggregateOptionType" FOREIGN KEY ("AggregateOptionTypeId") REFERENCES dbo."AggregateOptionType"("AggregateOptionTypeId");


--
-- TOC entry 4304 (class 2606 OID 21497)
-- Name: AggregateOption FK_AggregateOption_DomainAggregate; Type: FK CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."AggregateOption"
    ADD CONSTRAINT "FK_AggregateOption_DomainAggregate" FOREIGN KEY ("AggregateOptionId") REFERENCES dbo."DomainAggregate"("DomainAggregateId");


--
-- TOC entry 4317 (class 2606 OID 22287)
-- Name: AggregateSubmission FK_AggregateSubmission_DomainAggregate; Type: FK CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."AggregateSubmission"
    ADD CONSTRAINT "FK_AggregateSubmission_DomainAggregate" FOREIGN KEY ("DomainAggregateId") REFERENCES dbo."DomainAggregate"("DomainAggregateId");


--
-- TOC entry 4318 (class 2606 OID 22292)
-- Name: AggregateSubmission FK_AggregateSubmission_GenericSubmission; Type: FK CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."AggregateSubmission"
    ADD CONSTRAINT "FK_AggregateSubmission_GenericSubmission" FOREIGN KEY ("AggregateSubmissionId") REFERENCES dbo."GenericSubmission"("GenericSubmissionId") ON DELETE CASCADE;


--
-- TOC entry 4306 (class 2606 OID 21573)
-- Name: Association FK_Association_DomainAggregate; Type: FK CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."Association"
    ADD CONSTRAINT "FK_Association_DomainAggregate" FOREIGN KEY ("OtherAggregateId") REFERENCES dbo."OtherAggregate"("OtherAggregateId") ON DELETE CASCADE;


--
-- TOC entry 4307 (class 2606 OID 21578)
-- Name: Association FK_Association_OtherAggregate; Type: FK CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."Association"
    ADD CONSTRAINT "FK_Association_OtherAggregate" FOREIGN KEY ("DomainAggregateId") REFERENCES dbo."DomainAggregate"("DomainAggregateId");


--
-- TOC entry 4305 (class 2606 OID 21546)
-- Name: Child FK_Child_DomainAggregate; Type: FK CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."Child"
    ADD CONSTRAINT "FK_Child_DomainAggregate" FOREIGN KEY ("DomainAggregateId") REFERENCES dbo."DomainAggregate"("DomainAggregateId") ON DELETE CASCADE;


--
-- TOC entry 4319 (class 2606 OID 22319)
-- Name: CurrentAggregateSubmission FK_CurrentAggregateSubmission_DomainAggregate; Type: FK CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."CurrentAggregateSubmission"
    ADD CONSTRAINT "FK_CurrentAggregateSubmission_DomainAggregate" FOREIGN KEY ("DomainAggregateId") REFERENCES dbo."DomainAggregate"("DomainAggregateId");


--
-- TOC entry 4320 (class 2606 OID 22324)
-- Name: CurrentAggregateSubmission FK_CurrentAggregateSubmission_GenericSubmission; Type: FK CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."CurrentAggregateSubmission"
    ADD CONSTRAINT "FK_CurrentAggregateSubmission_GenericSubmission" FOREIGN KEY ("CurrentAggregateSubmissionId") REFERENCES dbo."GenericSubmission"("GenericSubmissionId");


--
-- TOC entry 4323 (class 2606 OID 22376)
-- Name: DateElement FK_DateElement_FieldValueElement; Type: FK CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."DateElement"
    ADD CONSTRAINT "FK_DateElement_FieldValueElement" FOREIGN KEY ("DateElementId") REFERENCES dbo."FieldValueElement"("FieldValueElementId") ON DELETE CASCADE NOT VALID;


--
-- TOC entry 4321 (class 2606 OID 22337)
-- Name: DomainAggregateFlagAttribute FK_DomainAggregateFlagAttribute_DomainAggregate; Type: FK CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."DomainAggregateFlagAttribute"
    ADD CONSTRAINT "FK_DomainAggregateFlagAttribute_DomainAggregate" FOREIGN KEY ("DomainAggregateId") REFERENCES dbo."DomainAggregate"("DomainAggregateId") ON DELETE CASCADE;


--
-- TOC entry 4322 (class 2606 OID 22342)
-- Name: DomainAggregateFlagAttribute FK_DomainAggregateFlagAttribute_FlagAttribute; Type: FK CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."DomainAggregateFlagAttribute"
    ADD CONSTRAINT "FK_DomainAggregateFlagAttribute_FlagAttribute" FOREIGN KEY ("FlagAttributeId") REFERENCES dbo."FlagAttribute"("FlagAttributeId");


--
-- TOC entry 4298 (class 2606 OID 21459)
-- Name: DomainAggregate FK_DomainAggregate_CategoryAttribute; Type: FK CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."DomainAggregate"
    ADD CONSTRAINT "FK_DomainAggregate_CategoryAttribute" FOREIGN KEY ("CategoryAttributeId") REFERENCES dbo."CategoryAttribute"("CategoryAttributeId");


--
-- TOC entry 4299 (class 2606 OID 21464)
-- Name: DomainAggregate FK_DomainAggregate_DomainIdentity; Type: FK CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."DomainAggregate"
    ADD CONSTRAINT "FK_DomainAggregate_DomainIdentity" FOREIGN KEY ("CreatedByDomainIdentityId") REFERENCES dbo."DomainIdentity"("DomainIdentityId");


--
-- TOC entry 4300 (class 2606 OID 21469)
-- Name: DomainAggregate FK_DomainAggregate_DomainIdentity1; Type: FK CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."DomainAggregate"
    ADD CONSTRAINT "FK_DomainAggregate_DomainIdentity1" FOREIGN KEY ("LastModifiedByDomainIdentityId") REFERENCES dbo."DomainIdentity"("DomainIdentityId");


--
-- TOC entry 4301 (class 2606 OID 21474)
-- Name: DomainAggregate FK_DomainAggregate_SubContainer; Type: FK CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."DomainAggregate"
    ADD CONSTRAINT "FK_DomainAggregate_SubContainer" FOREIGN KEY ("SubContainerId") REFERENCES dbo."SubContainer"("SubContainerId");


--
-- TOC entry 4302 (class 2606 OID 21479)
-- Name: DomainAggregate FK_DomainAggregate_Template; Type: FK CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."DomainAggregate"
    ADD CONSTRAINT "FK_DomainAggregate_Template" FOREIGN KEY ("TemplateId") REFERENCES dbo."Template"("TemplateId");


--
-- TOC entry 4313 (class 2606 OID 28209)
-- Name: FieldValueElement FK_FieldValueElement_FieldValue1; Type: FK CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."FieldValueElement"
    ADD CONSTRAINT "FK_FieldValueElement_FieldValue1" FOREIGN KEY ("FieldValueId") REFERENCES dbo."FieldValue"("FieldValueId") ON DELETE CASCADE NOT VALID;


--
-- TOC entry 4311 (class 2606 OID 21782)
-- Name: FieldValue FK_FieldValue_DomainIdentity; Type: FK CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."FieldValue"
    ADD CONSTRAINT "FK_FieldValue_DomainIdentity" FOREIGN KEY ("LastModifiedByDomainIdentifierId") REFERENCES dbo."DomainIdentity"("DomainIdentityId");


--
-- TOC entry 4312 (class 2606 OID 21787)
-- Name: FieldValue FK_FieldValue_Field; Type: FK CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."FieldValue"
    ADD CONSTRAINT "FK_FieldValue_Field" FOREIGN KEY ("FieldId") REFERENCES dbo."Field"("FieldId");


--
-- TOC entry 4324 (class 2606 OID 22371)
-- Name: FloatElement FK_FloatElement_FieldValueElement; Type: FK CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."FloatElement"
    ADD CONSTRAINT "FK_FloatElement_FieldValueElement" FOREIGN KEY ("FloatElementId") REFERENCES dbo."FieldValueElement"("FieldValueElementId") ON DELETE CASCADE;


--
-- TOC entry 4315 (class 2606 OID 22254)
-- Name: GenericSubmissionValue FK_GenericSubmissionValue_FieldValue; Type: FK CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."GenericSubmissionValue"
    ADD CONSTRAINT "FK_GenericSubmissionValue_FieldValue" FOREIGN KEY ("GenericSubmissionValueId") REFERENCES dbo."FieldValue"("FieldValueId") ON DELETE CASCADE;


--
-- TOC entry 4316 (class 2606 OID 22259)
-- Name: GenericSubmissionValue FK_GenericSubmissionValue_GenericSubmission; Type: FK CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."GenericSubmissionValue"
    ADD CONSTRAINT "FK_GenericSubmissionValue_GenericSubmission" FOREIGN KEY ("GenericSubmissionId") REFERENCES dbo."GenericSubmission"("GenericSubmissionId");


--
-- TOC entry 4314 (class 2606 OID 22241)
-- Name: GenericSubmission FK_GenericSubmission_DomainIdentity; Type: FK CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."GenericSubmission"
    ADD CONSTRAINT "FK_GenericSubmission_DomainIdentity" FOREIGN KEY ("SubmittedByDomainIdentifierId") REFERENCES dbo."DomainIdentity"("DomainIdentityId");


--
-- TOC entry 4325 (class 2606 OID 22386)
-- Name: IntegerElement FK_IntegerElement_FieldValueElement; Type: FK CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."IntegerElement"
    ADD CONSTRAINT "FK_IntegerElement_FieldValueElement" FOREIGN KEY ("IntegerElementId") REFERENCES dbo."FieldValueElement"("FieldValueElementId") ON DELETE CASCADE;


--
-- TOC entry 4326 (class 2606 OID 22399)
-- Name: MoneyElement FK_MoneyElement_FieldValueElement; Type: FK CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."MoneyElement"
    ADD CONSTRAINT "FK_MoneyElement_FieldValueElement" FOREIGN KEY ("MoneyElementId") REFERENCES dbo."FieldValueElement"("FieldValueElementId") ON DELETE CASCADE;


--
-- TOC entry 4297 (class 2606 OID 28217)
-- Name: OtherAggregate FK_OtherAggregate_AggregateOptionType; Type: FK CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."OtherAggregate"
    ADD CONSTRAINT "FK_OtherAggregate_AggregateOptionType" FOREIGN KEY ("AggregateOptionTypeId") REFERENCES dbo."AggregateOptionType"("AggregateOptionTypeId") NOT VALID;


--
-- TOC entry 4296 (class 2606 OID 21277)
-- Name: SubContainer FK_SubSubContainer_TopContainer; Type: FK CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."SubContainer"
    ADD CONSTRAINT "FK_SubSubContainer_TopContainer" FOREIGN KEY ("SubContainerId") REFERENCES dbo."TopContainer"("TopContainerId") ON DELETE CASCADE;


--
-- TOC entry 4327 (class 2606 OID 22412)
-- Name: TextElement FK_TextElement_FieldValueElement; Type: FK CONSTRAINT; Schema: dbo; Owner: -
--

ALTER TABLE ONLY dbo."TextElement"
    ADD CONSTRAINT "FK_TextElement_FieldValueElement" FOREIGN KEY ("TextElementId") REFERENCES dbo."FieldValueElement"("FieldValueElementId") ON DELETE CASCADE;


-- Completed on 2022-04-01 12:39:19

--
-- PostgreSQL database dump complete
--

