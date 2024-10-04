
using DonNetTurorialProject1.Models.Entities;

namespace DonNetTurorialProject1.Data.Mapping
{
    public class BorrowBookMapper : Profile
    {
        public BorrowBookMapper()
        {
            //CreateMap<BorrowRecord, BorrowViewModel>().PreserveReferences();
            //CreateMap<BorrowViewModel, BorrowRecord>().PreserveReferences();
            //CreateMap<BorrowRecord, ReturnViewModel>().PreserveReferences();
            //CreateMap<ReturnViewModel, BorrowRecord>().PreserveReferences();
            // Map properties from BorrowRecord and nested Book to BorrowRecordDetailsDto
            CreateMap<BorrowRecord, BorrowViewModel>()
                .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book.Title))
                .ForMember(dest => dest.BookId, opt => opt.MapFrom(src => src.Book.BookId))
                .ForMember(dest => dest.BorrowerEmail, opt => opt.MapFrom(src => src.BorrowerEmail))
                .ForMember(dest => dest.BorrowerName, opt => opt.MapFrom(src => src.BorrowerName))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone)).PreserveReferences().ReverseMap();

            CreateMap<BorrowRecord, ReturnViewModel>()
                .ForMember(dest => dest.BorrowRecordId, opt => opt.MapFrom(src => src.BorrowRecordId))
                .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book.Title))
                .ForMember(dest => dest.BorrowerName, opt => opt.MapFrom(src => src.BorrowerName))
                .ForMember(dest => dest.BorrowDate, opt => opt.MapFrom(src => src.BorrowDate)).PreserveReferences().ReverseMap(); ;
               
        }
    }
}