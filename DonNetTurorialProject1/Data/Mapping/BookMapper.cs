
using DonNetTurorialProject1.Models.Entities;

namespace DonNetTurorialProject1.Data.Mapping
{
    public class BookMapper : Profile
    {
        public BookMapper()
        {
            //CreateMap<Book, CreateOrUpdateBookViewModel>().PreserveReferences();
            //CreateMap<CreateOrUpdateBookViewModel, Book>().PreserveReferences();
            //CreateMap<Book, BookReponseViewModel>().PreserveReferences();
            //CreateMap<BookReponseViewModel, Book>().PreserveReferences();

            CreateMap<Book, BorrowViewModel>()
                    .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Title))
                    .ForMember(dest => dest.BookId, opt => opt.MapFrom(src => src.BookId)).PreserveReferences().ReverseMap();
        }
    }
}