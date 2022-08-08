create database hashserverdb;

\c hashserverdb

drop table if exists hashes;
drop table if exists messages;

create table if not exists hashes(
    id uuid primary key,
    hash text not null unique
);

create table if not exists messages(
    id uuid primary key,
    hashid uuid not null,
    constraint fk_messages_hashes foreign key(hashid) references hashes(id) on delete cascade
);